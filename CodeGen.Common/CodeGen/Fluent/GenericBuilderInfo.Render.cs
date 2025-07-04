using System.Diagnostics.CodeAnalysis;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class GenericBuilderInfoExtensions
{
   internal static void RenderGenericParameter(this ref CodeBuilder builder, int index)
   {
      var first = true;
      builder.Writer.Write("<");
      
      foreach (var generic in builder.GetTemporaryEnumerator<GenericBuilderInfo>(index))
      {
         if (!first)
         {
            builder.Writer.Write(", ");
         }
         
         builder.Writer.Write(generic.Name.Span);
         first = false;
      }
      
      builder.Writer.Write(">");
   }

   internal static void RenderGenericParameter(
      this ref CodeBuilder builder,
      Span<byte> region,
      int offset,
      int length)
   {
      var first = true;
      var offsetEnd = offset + length;
      
      region = region[offset..];
      builder.Writer.Write("<");

      while (offset < offsetEnd)
      {
         var consumed = GenericBuilderInfo.Read(region, out var generic);
         
         region = region[consumed..];
         offset += consumed;
         
         if (!first)
         {
            builder.Writer.Write(", ");
         }
         
         builder.Writer.Write(generic.Name.Span);
         
         first = false;
      }
      
      builder.Writer.Write(">");
   }

   internal static void RenderGenericConstraints(this ref CodeBuilder builder, int index)
   {
      var enumerator = builder.GetTemporaryEnumerator<GenericBuilderInfo>(index).GetEnumerator();
      var span = builder.GetTemporarySpan(index);

      while (enumerator.MoveNext())
      {
         var generic = enumerator.Current;
         if (generic.ConstraintCount <= 0) continue;
         
         builder.Writer.Write("where ");
         builder.Writer.Write(generic.Name.Span);
         builder.Writer.Write(" : ");

         var offset = enumerator.CurrentOffset + generic.Offset;
         
         for (var e = 0; e < generic.ConstraintCount; e++)
         {
            var consumed = RefStringView.Read(span[offset..], out var constraint);
            offset += consumed;

            if (e > 0)
            {
               builder.Writer.Write(", ");
            }
            
            builder.Writer.Write(constraint.Span);
         }

         builder.Writer.WriteLine();
      }
   }
   
   internal static void RenderGenericConstraints(
      this ref CodeBuilder builder,
      Span<byte> region,
      int offset,
      int length)
   {
      var offsetEnd = offset + length;
      region = region[offset..];

      while (offset < offsetEnd)
      {
         var consumed = GenericBuilderInfo.Read(region, out var generic);
         if (generic.ConstraintCount <= 0)
         {
            region = region[consumed..];
            offset += consumed;
            continue;
         }
         
         builder.Writer.Write("where ");
         builder.Writer.Write(generic.Name.Span);
         builder.Writer.Write(" : ");
         
         var iOffset = generic.Offset;
         
         for (var e = 0; e < generic.ConstraintCount; e++)
         {
            var iConsumed = RefStringView.Read(region[iOffset..], out var constraint);
            iOffset += iConsumed;
            offset += iConsumed;

            if (e > 0)
            {
               builder.Writer.Write(", ");
            }
            
            builder.Writer.Write(constraint.Span);
         }

         builder.Writer.WriteLine();
         
         region = region[consumed..];
         offset += consumed;
      }
   }
}