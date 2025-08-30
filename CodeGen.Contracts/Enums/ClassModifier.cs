using CodeGen.Contracts.Extensions;

namespace CodeGen.Contracts.Enums;

[Flags]
public enum ClassModifier : ushort
{
   None = 0,
   Abstract = 1 << 0,
   Sealed = 1 << 1,
   Static = 1 << 2,
   Partial = 1 << 3,
   Unsafe = 1 << 4,
   New = 1 << 5,
   Record = 1 << 6,
}

public static class ClassModifierExtensions
{
   extension(ClassModifier modifier)
   {
      public void FillCharBuffer(scoped in Span<char> buffer)
      {
         var offset = 0;
         
         if (modifier.HasFlag(ClassModifier.New)) New.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Static)) Static.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Sealed)) Sealed.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Abstract)) Abstract.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Unsafe)) Unsafe.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Partial)) Partial.AsSpan().WriteToBuffer(buffer, ref offset);
         if (modifier.HasFlag(ClassModifier.Record)) Record.AsSpan().WriteToBuffer(buffer, ref offset);
      }

      public int GetCharBufferSize()
      {
         var length = 0;

         if (modifier.HasFlag(ClassModifier.Static)) length += Static.Length + 1;
         if (modifier.HasFlag(ClassModifier.Abstract)) length += Abstract.Length + 1;
         if (modifier.HasFlag(ClassModifier.Sealed)) length += Sealed.Length + 1;
         if (modifier.HasFlag(ClassModifier.Partial)) length += Partial.Length + 1;
         if (modifier.HasFlag(ClassModifier.Unsafe)) length += Unsafe.Length + 1;
         if (modifier.HasFlag(ClassModifier.New)) length += New.Length + 1;
         if (modifier.HasFlag(ClassModifier.Record)) length += Record.Length + 1;

         return Math.Max(0, length - 1);
      }
   }
   
   private const string Abstract = "abstract";
   private const string Sealed = "sealed";
   private const string Static = "static";
   private const string Partial = "partial";
   private const string Unsafe = "unsafe";
   private const string New = "new";
   private const string Record = "record";
}
