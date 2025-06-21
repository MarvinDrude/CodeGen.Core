using CodeGen.Common.Buffers;

namespace CodeGen.Core.Tests.Buffers;

public class ByteWriterReaderSimpleTests
{
   [Test]
   public async Task SimpleNumbers()
   {
      const short v1 = 232;
      const int v2 = 454354;
      const long v3 = 34324332432;
      const float v4 = 333.333f;
      const double v5 = 555555.31d;

      var writer = new ByteWriter(
         stackalloc byte[256]);
      
      writer.WriteBigEndian(v1);
      writer.WriteLittleEndian(v1);
      
      writer.WriteBigEndian(v2);
      writer.WriteLittleEndian(v2);
      
      writer.WriteBigEndian(v3);
      writer.WriteLittleEndian(v3);
      
      writer.WriteBigEndian(v4);
      writer.WriteLittleEndian(v4);
      
      writer.WriteBigEndian(v5);
      writer.WriteLittleEndian(v5);

      var reader = new ByteReader(writer.WrittenSpan);

      var r1b = reader.ReadBigEndian<short>();
      var r1l = reader.ReadLittleEndian<short>();
      
      var r2b = reader.ReadBigEndian<int>();
      var r2l = reader.ReadLittleEndian<int>();
      
      var r3b = reader.ReadBigEndian<long>();
      var r3l = reader.ReadLittleEndian<long>();
      
      var r4b = reader.ReadBigEndian<float>();
      var r4l = reader.ReadLittleEndian<float>();
      
      var r5b = reader.ReadBigEndian<double>();
      var r5l = reader.ReadLittleEndian<double>();

      writer.Dispose();
      await Assert.That(reader.BytesLeft).IsEqualTo(0);
      
      await Assert.That(r1b).IsEqualTo(v1);
      await Assert.That(r1l).IsEqualTo(v1);
      
      await Assert.That(r2b).IsEqualTo(v2);
      await Assert.That(r2l).IsEqualTo(v2);
      
      await Assert.That(r3b).IsEqualTo(v3);
      await Assert.That(r3l).IsEqualTo(v3);
      
      await Assert.That(r4b).IsEqualTo(v4);
      await Assert.That(r4l).IsEqualTo(v4);
      
      await Assert.That(r5b).IsEqualTo(v5);
      await Assert.That(r5l).IsEqualTo(v5);
   }
}