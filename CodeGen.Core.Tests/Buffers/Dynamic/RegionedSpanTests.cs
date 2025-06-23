using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Core.Tests.Buffers.Dynamic;

public class RegionedSpanTests
{
   [Test]
   public async Task AddRegionSimpleTests()
   {
      var regioned = new RegionedSpan(stackalloc byte[256]);
      regioned.AddRegion<int>(60);

      
   }
}