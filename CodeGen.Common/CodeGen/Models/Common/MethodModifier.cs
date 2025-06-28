using System.Runtime.CompilerServices;

namespace CodeGen.Common.CodeGen.Models.Common;

[Flags]
public enum MethodModifier
{
   None = 0,
   Static = 1 << 0,
   Extern = 1 << 1,
   Unsafe = 1 << 2,
   Async = 1 << 3,
   Partial = 1 << 4,
   Override = 1 << 5,
   Abstract = 1 << 6,
   Virtual = 1 << 7,
   New = 1 << 8,
   Sealed = 1 << 9,
   
   Ref = 1 << 10,
   ReadOnly = 1 << 11,
}

public static class MethodModifierExtensions
{
   public static void FillCharBuffer(this MethodModifier modifier, scoped in Span<char> buffer)
   {
      var offset = 0;

      if (modifier.HasFlag(MethodModifier.New)) WriteCharBuffer("new", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Static)) WriteCharBuffer("static", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.ReadOnly)) WriteCharBuffer("readonly", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Virtual)) WriteCharBuffer("virtual", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Abstract)) WriteCharBuffer("abstract", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Override)) WriteCharBuffer("override", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Sealed)) WriteCharBuffer("sealed", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Extern)) WriteCharBuffer("extern", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Partial)) WriteCharBuffer("partial", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Async)) WriteCharBuffer("async", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Unsafe)) WriteCharBuffer("unsafe", buffer, ref offset);
      if (modifier.HasFlag(MethodModifier.Ref)) WriteCharBuffer("ref", buffer, ref offset);
   }

   private static void WriteCharBuffer(
      scoped ReadOnlySpan<char> text,
      scoped Span<char> buffer,
      ref int offset)
   {
      text.CopyTo(buffer[offset..]);
      offset += text.Length;

      if (offset < buffer.Length)
         buffer[offset++] = ' ';
   }

   public static int GetCharBufferSize(this MethodModifier modifier)
   {
      var size = 0;

      if (modifier.HasFlag(MethodModifier.New)) size += 3 + 1;
      if (modifier.HasFlag(MethodModifier.Static)) size += 6 + 1;
      if (modifier.HasFlag(MethodModifier.Virtual)) size += 7 + 1;
      if (modifier.HasFlag(MethodModifier.Abstract)) size += 8 + 1;
      if (modifier.HasFlag(MethodModifier.Override)) size += 8 + 1;
      if (modifier.HasFlag(MethodModifier.Sealed)) size += 6 + 1;
      if (modifier.HasFlag(MethodModifier.Extern)) size += 6 + 1;
      if (modifier.HasFlag(MethodModifier.Partial)) size += 7 + 1;
      if (modifier.HasFlag(MethodModifier.Async)) size += 5 + 1;
      if (modifier.HasFlag(MethodModifier.Unsafe)) size += 6 + 1;
      if (modifier.HasFlag(MethodModifier.ReadOnly)) size += 8 + 1;
      if (modifier.HasFlag(MethodModifier.Ref)) size += 3 + 1;

      return size == 0 ? 0 : size - 1;
   }
}
