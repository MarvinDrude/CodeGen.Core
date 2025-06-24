using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Core.Tests.Buffers.Dynamic;

public class RegionedSpanTests
{
   [Test]
   public async Task AddRegionSimpleTests()
   {
      var regioned = new RegionedSpan(stackalloc byte[256]);
      var index1 = regioned.AddRegion(60);
      var index2 = regioned.AddRegion(90);

      var span1 = regioned.GetRegion(index1);
      var span2 = regioned.GetRegion(index2);

      var span1length = span1.Length;
      var span2length = span2.Length;

      await Assert.That(span1length).IsEqualTo(60);
      await Assert.That(span2length).IsEqualTo(90);
   }
   
   [Test]
   public async Task AddRegionResizeSimpleTests()
   {
      var regioned = new RegionedSpan(stackalloc byte[150]);
      var index1 = regioned.AddRegion(60);
      var index2 = regioned.AddRegion(90);

      var span1 = regioned.GetRegion(index1);
      var span2 = regioned.GetRegion(index2);

      var span1length = span1.Length;
      var span2length = span2.Length;

      await Assert.That(span1length).IsEqualTo(60);
      await Assert.That(span2length).IsEqualTo(90);
   }
   
   [Test]
   public async Task AddRegionResizeHeaderSimpleTests()
   {
      var regioned = new RegionedSpan(stackalloc byte[150]);
      List<int> indexes = [];
      List<int> sizes = [];

      for (var e = 0; e < 17; e++)
      {
         indexes.Add(regioned.AddRegion((e + 1) * 10));
      }

      foreach (var index in indexes)
      {
         sizes.Add(regioned.GetRegion(index).Length);
      }

      for (var e = 0; e < 17; e++)
      {
         var targetSize = (e + 1) * 10;
         await Assert.That(sizes[e]).IsEqualTo(targetSize);
      }
   }
   
   [Test]
   public async Task ResizeRegionSimpleTests()
   {
      var regioned = new RegionedSpan(stackalloc byte[150]);

      var index1 = regioned.AddRegion(5);
      var index2 = regioned.AddRegion(10);
      
      regioned.ResizeRegion(index1, 60);

      var span1 = regioned.GetRegion(index1);
      var span2 = regioned.GetRegion(index2);

      var index3 = regioned.AddRegion(6);
      var span3 = regioned.GetRegion(index3);
      
      var span1length = span1.Length;
      var span2length = span2.Length;
      var span3length = span3.Length;
      
      await Assert.That(span1length).IsEqualTo(60);
      await Assert.That(span2length).IsEqualTo(10);
      await Assert.That(span3length).IsEqualTo(6);
   }
}