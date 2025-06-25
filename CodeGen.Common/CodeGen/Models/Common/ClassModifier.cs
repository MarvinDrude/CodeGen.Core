namespace CodeGen.Common.CodeGen.Models.Common;

[Flags]
public enum ClassModifier
{
   None = 0,
   Abstract = 1 << 0,
   Sealed = 1 << 1,
   Static = 1 << 2,
   Partial = 1 << 3,
   Unsafe = 1 << 4
}

public static class ClassModifierExtensions
{
   public static void FillCharBuffer(this ClassModifier modifier, scoped in Span<char> buffer)
   {
      var offset = 0;
      
      if (modifier.HasFlag(ClassModifier.Unsafe)) WriteCharBuffer("unsafe", buffer, ref offset);
      if (modifier.HasFlag(ClassModifier.Static)) WriteCharBuffer("static", buffer, ref offset);
      if (modifier.HasFlag(ClassModifier.Abstract)) WriteCharBuffer("abstract", buffer, ref offset);
      if (modifier.HasFlag(ClassModifier.Sealed)) WriteCharBuffer("sealed", buffer, ref offset);
      if (modifier.HasFlag(ClassModifier.Partial)) WriteCharBuffer("partial", buffer, ref offset);
   }

   private static void WriteCharBuffer(
      scoped ReadOnlySpan<char> text, 
      scoped Span<char> buffer,
      ref int offset)
   {
      text.CopyTo(buffer[offset..]);
      offset += text.Length;

      if (offset < buffer.Length)
      {
         buffer[offset++] = ' ';
      }
   }
   
   public static int GetCharBufferSize(this ClassModifier modifier)
   {
      var size = 0;

      if (modifier.HasFlag(ClassModifier.Abstract)) size += 8 + 1;
      if (modifier.HasFlag(ClassModifier.Sealed)) size += 6 + 1;
      if (modifier.HasFlag(ClassModifier.Static)) size += 6 + 1;
      if (modifier.HasFlag(ClassModifier.Partial)) size += 7 + 1;
      if (modifier.HasFlag(ClassModifier.Unsafe)) size += 6 + 1;
      
      return size == 0 ? 0 : size - 1;
   }
}