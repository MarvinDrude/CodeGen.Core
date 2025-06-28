using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ParameterBuilderInfoExtensions
{
   internal static void RenderParameters(
      this ref CodeBuilder builder,
      Span<byte> span,
      int offset,
      int length)
   {
      var offsetEnd = offset + length;
      var first = true;

      span = span[offset..];
      
      while (offset < offsetEnd)
      {
         var consumed = ParameterBuilderInfo.Read(span, out var instance);
         
         span = span[consumed..];
         offset += consumed;

         if (!first)
         {
            builder.Writer.WriteLine(", ");
         }
         first = false;
         
         if (instance.Attributes.Length > 0)
         {
            builder.Writer.Write(instance.Attributes.Span);
            builder.Writer.Write(" ");
         }

         var lengthModifiers = instance.Modifiers.GetCharBufferSize();

         if (lengthModifiers > 0)
         {
            var modifiersSpan = builder.Writer.AcquireSpan(lengthModifiers);
            instance.Modifiers.FillCharBuffer(modifiersSpan);
            
            builder.Writer.Write(" ");
         }
         
         builder.Writer.Write(instance.TypeName.Span);
         builder.Writer.Write(" ");
         builder.Writer.Write(instance.Name.Span);
      }
   }
}