namespace CodeGen.Common.CodeGen.Models.Common;

[Flags]
public enum ParameterModifier
{
   None = 0,
   This = 1 << 0,
   Ref = 1 << 1,
   Out = 1 << 2,
   In = 1 << 3,
   Scoped = 1 << 4,
   Params = 1 << 5,
   ReadOnly = 1 << 6
}

public static class ParameterModifierExtensions
{
   public static void FillCharBuffer(this ParameterModifier modifier, scoped in Span<char> buffer)
   {
      var offset = 0;

      if (modifier.HasFlag(ParameterModifier.This))   WriteCharBuffer("this", buffer, ref offset);
      if (modifier.HasFlag(ParameterModifier.Params)) WriteCharBuffer("params", buffer, ref offset);
      if (modifier.HasFlag(ParameterModifier.Scoped)) WriteCharBuffer("scoped", buffer, ref offset);

      if (modifier.HasFlag(ParameterModifier.ReadOnly) && modifier.HasFlag(ParameterModifier.Ref))
         WriteCharBuffer("readonly", buffer, ref offset);

      if (modifier.HasFlag(ParameterModifier.Ref))      WriteCharBuffer("ref", buffer, ref offset);
      else if (modifier.HasFlag(ParameterModifier.Out)) WriteCharBuffer("out", buffer, ref offset);
      else if (modifier.HasFlag(ParameterModifier.In))  WriteCharBuffer("in", buffer, ref offset);
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
   
   public static int GetCharBufferSize(this ParameterModifier modifier)
   {
      var size = 0;

      if (modifier.HasFlag(ParameterModifier.This))   size += 4 + 1;
      if (modifier.HasFlag(ParameterModifier.Params)) size += 6 + 1;
      if (modifier.HasFlag(ParameterModifier.Scoped)) size += 6 + 1;
      if (modifier.HasFlag(ParameterModifier.ReadOnly) 
          && modifier.HasFlag(ParameterModifier.Ref)) size += 8 + 1;

      if ((modifier.HasFlag(ParameterModifier.Ref) && !modifier.HasFlag(ParameterModifier.ReadOnly))
          || modifier.HasFlag(ParameterModifier.Out)) size += 3 + 1;
      else if (modifier.HasFlag(ParameterModifier.In)) size += 2 + 1;

      return size == 0 ? 0 : size - 1;
   }
}