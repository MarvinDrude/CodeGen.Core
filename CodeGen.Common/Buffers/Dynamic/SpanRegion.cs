namespace CodeGen.Common.Buffers.Dynamic;

public ref struct SpanRegion
{
   public int Offset;
   public int ByteLength;

   private Span<byte> _region;
   
   public SpanRegion(
      int offset,
      Span<byte> region)
   {
      _region = region;
      
      Offset = offset;
      ByteLength = region.Length;
   }
}