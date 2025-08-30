namespace CodeGen.Contracts.Extensions;

public static class ReadOnlySpanExtensions
{
   extension(scoped in ReadOnlySpan<char> span)
   {
      /// <summary>
      /// Writes to buffer and adds a space at the end if space is available.
      /// </summary>
      public void WriteToBuffer(scoped in Span<char> buffer, ref int offset)
      {
         span.CopyTo(buffer[offset..]);
         offset += span.Length;

         if (offset < buffer.Length)
         {
            buffer[offset++] = ' ';
         }
      }
   }
}