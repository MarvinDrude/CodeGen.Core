namespace CodeGen.Common.Buffers.Dynamic;

public ref struct SpanRegion
{
   public int Offset;
   public int ByteLength;
   public int InnerOffset;
   public int ItemCount;

   public SpanRegion(
      int offset,
      int byteLength,
      int innerOffset,
      int itemCount)
   {
      Offset = offset;
      ByteLength = byteLength;
      InnerOffset = innerOffset;
      ItemCount = itemCount;
   }
}