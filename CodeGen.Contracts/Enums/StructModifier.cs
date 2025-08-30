using CodeGen.Contracts.Extensions;

namespace CodeGen.Contracts.Enums;

[Flags]
public enum StructModifier
{
   None = 0,
   Partial = 1 << 1,
   Unsafe = 1 << 2,
   Record = 1 << 3,
   ReadOnly = 1 << 4,
   Ref = 1 << 5,
   New = 1 << 6,
}

public static class StructModifierExtensions
{
   extension(ClassModifier modifier)
   {
      public void FillCharBuffer(scoped in Span<char> buffer)
      {
         var offset = 0;
         
         if (modifier.HasFlag(StructModifier.New)) New.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(StructModifier.ReadOnly)) ReadOnly.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(StructModifier.Unsafe)) Unsafe.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(StructModifier.Ref)) Ref.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(StructModifier.Partial)) Partial.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(StructModifier.Record)) Record.AsSpan().WriteToBuffer(buffer, ref offset);
      }

      public int GetCharBufferSize()
      {
         var length = 0;

         if (modifier.HasFlag(StructModifier.ReadOnly)) length += ReadOnly.Length + 1;
         if (modifier.HasFlag(StructModifier.Ref)) length += Ref.Length + 1;
         if (modifier.HasFlag(StructModifier.Partial)) length += Partial.Length + 1;
         if (modifier.HasFlag(StructModifier.Unsafe)) length += Unsafe.Length + 1;
         if (modifier.HasFlag(StructModifier.New)) length += New.Length + 1;
         if (modifier.HasFlag(StructModifier.Record)) length += Record.Length + 1;

         return Math.Max(0, length - 1);
      }
   }

   private const string Ref = "ref";
   private const string ReadOnly = "readonly";
   private const string Partial = "partial";
   private const string Unsafe = "unsafe";
   private const string New = "new";
   private const string Record = "record";
}

public new readonly unsafe ref partial struct Test
{
   
}