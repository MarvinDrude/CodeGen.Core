using System.Diagnostics.CodeAnalysis;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class GenericBuilderInfoExtensions
{
   public static void RenderGenericParameter(this ref CodeBuilder builder, int index)
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

   public static void RenderGenericConstraints(this ref CodeBuilder builder, int index)
   {
      var enumerator = builder.GetTemporaryEnumerator<GenericBuilderInfo>(index).GetEnumerator();
      var span = builder.GetTemporarySpan(index);

      while (enumerator.MoveNext())
      {
         var generic = enumerator.Current;
         
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
}