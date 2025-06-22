using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Core.Tests.Buffers.Dynamic;

public class PackedBoolsTests
{
   [Test]
   public async Task Simple()
   {
      var packed = new PackedBools();
      
      packed.Set(0, true);
      var shouldTrue1 = packed.Get(0);
      var shouldFalse1 = packed.Get(1);
      var shouldFalse2 = packed.Get(7);

      packed.Set(5, true);
      var shouldTrue2 = packed.Get(5);
      var shouldFalse3 = packed.Get(6);
      var shouldFalse4 = packed.Get(3);
      
      await Assert.That(shouldTrue1).IsEqualTo(true);
      await Assert.That(shouldFalse1).IsEqualTo(false);
      await Assert.That(shouldFalse2).IsEqualTo(false);
      await Assert.That(shouldTrue2).IsEqualTo(true);
      await Assert.That(shouldFalse3).IsEqualTo(false);
      await Assert.That(shouldFalse4).IsEqualTo(false);
   }
}