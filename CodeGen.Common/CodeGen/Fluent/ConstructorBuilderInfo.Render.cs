using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ConstructorBuilderInfoExtensions
{
   internal static ref CodeBuilder RenderConstructors(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      var enumerator = builder.GetTemporaryEnumerator<ConstructorBuilderInfo>(builder.RegionIndexConstructors).GetEnumerator();
      var span = builder.GetTemporarySpan(builder.RegionIndexConstructors);
      
      while (enumerator.MoveNext())
      {
         RenderConstructor(
            ref builder, 
            ref info,
            span,
            enumerator.Current, 
            enumerator.CurrentOffset);
      }
      
      return ref builder;
   }

   internal static void RenderConstructor(
      ref CodeBuilder builder, 
      ref ClassBuilderInfo classBuilder,
      Span<byte> span,
      scoped ConstructorBuilderInfo info, 
      int offset)
   {
      var accessLength = info.AccessModifier.GetCharBufferSize();

      if (accessLength > 0)
      {
         var accessSpan = builder.Writer.AcquireSpanIndented(accessLength + 1);
         info.AccessModifier.FillCharBuffer(accessSpan);

         accessSpan[^1] = ' ';
      }

      if (info.IsStatic)
      {
         builder.Writer.Write("static ");
      }
      
      builder.Writer.Write(classBuilder.Name.Span);

      if (info.ParameterLength > 0)
      {
         builder.Writer.WriteLine("(");
         builder.Writer.UpIndent();
         
         builder.RenderParameters(span, offset + info.ParameterOffset, info.ParameterLength);

         builder.Writer.WriteLine(info.IsSignatureOnly ? ");" : ")");
         builder.Writer.DownIndent();
      }
      else
      {
         builder.Writer.WriteLine("()");
      }

      if (info.HasBaseCall || info.HasThisCall)
      {
         builder.Writer.UpIndent();
         builder.Writer.Write(info.HasBaseCall ? ": base(" : ": this(");

         if (info.BaseParameterLength > 0)
         {
            var first = true;
            var bOffset = info.BaseParameterOffset;
            var bOffsetEnd = bOffset + info.BaseParameterLength;
            
            span = span[bOffset..];

            while (bOffset < bOffsetEnd)
            {
               var consumed = RefStringView.Read(span, out var baseParameter);
               
               span = span[consumed..];
               bOffset += consumed;
               
               if (!first)
               {
                  builder.Writer.Write(", ");
               }
               first = false;
               
               builder.Writer.Write(baseParameter.Span);
            }
         }

         builder.Writer.WriteLine(")");
         builder.Writer.DownIndent();
      }

      builder.Writer.OpenBody();
   }
}