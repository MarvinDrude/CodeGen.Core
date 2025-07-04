using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class MethodBuilderInfoExtensions
{
   internal static ref CodeBuilder RenderMethods(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;
      
      var enumerator = builder.GetTemporaryEnumerator<MethodBuilderInfo>(builder.RegionIndexMethods).GetEnumerator();
      var span = builder.GetTemporarySpan(builder.RegionIndexMethods);

      while (enumerator.MoveNext())
      {
         RenderMethod(
            ref builder,
            ref info,
            span,
            enumerator.Current,
            enumerator.CurrentOffset);
      }
      
      return ref builder;
   }

   internal static void RenderMethod(
      ref CodeBuilder builder,
      ref ClassBuilderInfo classBuilder,
      Span<byte> span,
      scoped MethodBuilderInfo info,
      int offset)
   {
      var accessLength = info.AccessModifier.GetCharBufferSize();

      if (accessLength > 0)
      {
         var accessSpan = builder.Writer.AcquireSpanIndented(accessLength + 1);
         info.AccessModifier.FillCharBuffer(accessSpan);

         accessSpan[^1] = ' ';
      }

      var modifierLength = info.Modifiers.GetCharBufferSize();

      if (modifierLength > 0)
      {
         var modifierSpan = builder.Writer.AcquireSpanIndented(modifierLength + 1);
         info.Modifiers.FillCharBuffer(modifierSpan);

         modifierSpan[^1] = ' ';
      }
      
      builder.Writer.Write(info.ReturnType.Span);
      builder.Writer.Write(" ");
      builder.Writer.Write(info.Name.Span);

      if (info.GenericParameterLength > 0)
      {
         builder.RenderGenericParameter(span, info.GenericParameterOffset, info.GenericParameterLength);
      }

      if (info.ParameterLength > 0)
      {
         builder.Writer.WriteLine("(");
         builder.Writer.UpIndent();
         
         builder.RenderParameters(span, offset + info.ParameterOffset, info.ParameterLength);

         builder.Writer.Write(")");
         builder.Writer.DownIndent();
      }
      else
      {
         builder.Writer.Write("()");
      }

      if (info.GenericParameterLength > 0)
      {
         builder.Writer.UpIndent();
         builder.Writer.WriteLine();
         builder.RenderGenericConstraints(span, offset + info.GenericParameterOffset, info.GenericParameterLength);
         builder.Writer.DownIndent();
      }

      if (info.IsSignatureOnly)
      {
         builder.Writer.WriteLine(";");
      }
      else
      {
         builder.Writer.OpenBody();
      }
   }
}