using CodeGen.Contracts.Extensions;

namespace CodeGen.Contracts.Enums;

[Flags]
public enum InterfaceModifier : ushort
{
   None = 0,
   New = 1 << 1,
   Unsafe = 1 << 2,
   Partial = 1 << 3
}

public static class InterfaceModifierExtensions
{
   extension(InterfaceModifier modifier)
   {
      public void FillCharBuffer(scoped in Span<char> buffer)
      {
         var offset = 0;
         
         if (modifier.HasFlag(InterfaceModifier.New)) New.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(InterfaceModifier.Unsafe)) Unsafe.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(InterfaceModifier.Partial)) Partial.AsSpan().WriteToBuffer(buffer, ref offset);
      }

      public int GetCharBufferSize()
      {
         var length = 0;
         
         if (modifier.HasFlag(InterfaceModifier.New)) length += New.Length + 1;
         if (modifier.HasFlag(InterfaceModifier.Unsafe)) length += Unsafe.Length + 1;
         if (modifier.HasFlag(InterfaceModifier.Partial)) length += Partial.Length + 1;

         return Math.Max(0, length - 1);
      }
   }
   
   private const string New = "new";
   private const string Unsafe = "unsafe";
   private const string Partial = "partial";
}