using CodeGen.Common.Buffers;

namespace CodeGen.Core.Tests.Buffers;

public class RefStringViewTests
{
   [Test]
   public async Task SimpleSerialize()
   {
      const string str = "Hallo!";
      
      var view = new RefStringView(str);
      Span<byte> buffer = stackalloc byte[RefStringView.CalculateByteLength(in view)];

      RefStringView.Write(buffer, in view);
      RefStringView.Read(buffer, out var newView);

      var newString = new string(newView.Span);
      
      await Assert.That(newString).IsEqualTo(str);
   }
}