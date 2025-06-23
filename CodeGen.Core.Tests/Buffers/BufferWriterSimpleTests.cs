using CodeGen.Common.Buffers;

namespace CodeGen.Core.Tests.Buffers;

public class BufferWriterSimpleTests
{
   [Test]
   public async Task MoveNoOverlapNoResizeStack()
   {
      var writer = new BufferWriter<byte>(
         stackalloc byte[512]);
      
      writer.Write([1, 2, 3, 4, 5]);
      writer.Move(0, 5, 5);
      
      var buffer = writer.WrittenSpan.ToArray();
      byte[] result = [1, 2, 3, 4, 5, 1, 2, 3, 4, 5];
      writer.Dispose();

      await Assert.That(result.SequenceEqual(buffer)).IsEqualTo(true);
   }
   
   [Test]
   public async Task MoveNoOverlapResizeStack()
   {
      var writer = new BufferWriter<byte>(
         stackalloc byte[6]);
      
      writer.Write([1, 2, 3, 4, 5]);
      writer.Move(0, 5, 5);
      
      var buffer = writer.WrittenSpan.ToArray();
      byte[] result = [1, 2, 3, 4, 5, 1, 2, 3, 4, 5];
      writer.Dispose();

      await Assert.That(result.SequenceEqual(buffer)).IsEqualTo(true);
   }
   
   [Test]
   public async Task MoveOverlapNoResizeStack()
   {
      var writer = new BufferWriter<byte>(
         stackalloc byte[8]);
      
      writer.Write([1, 2, 3, 4, 5]);
      writer.Move(0, 5, 3);
      
      var buffer = writer.WrittenSpan.ToArray();
      byte[] result = [1, 2, 3, 1, 2, 3, 4, 5];
      writer.Dispose();

      await Assert.That(result.SequenceEqual(buffer)).IsEqualTo(true);
   }
   
   [Test]
   public async Task MoveOverlapResizeStack()
   {
      var writer = new BufferWriter<byte>(
         stackalloc byte[5]);
      
      writer.Write([1, 2, 3, 4, 5]);
      writer.Move(0, 5, 3);
      
      var buffer = writer.WrittenSpan.ToArray();
      byte[] result = [1, 2, 3, 1, 2, 3, 4, 5];
      writer.Dispose();

      await Assert.That(result.SequenceEqual(buffer)).IsEqualTo(true);
   }
}