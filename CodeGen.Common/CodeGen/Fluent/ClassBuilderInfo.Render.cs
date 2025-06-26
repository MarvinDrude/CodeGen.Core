using System.Reflection.Emit;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Immediate;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ClassBuilderInfoExtensions
{
   public static ref CodeBuilder Render(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      if (!info.IsHeaderRendered)
      {
         RenderHeader(ref info);
         info.IsHeaderRendered = true;
      }
      
      return ref info.Builder;
   }

   internal static void RenderHeader(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      var accessLength = info.AccessModifier.GetCharBufferSize();
      var modifiersLength = info.ClassModifiers.GetCharBufferSize();

      if (accessLength > 0)
      {
         var accessSpan = builder.Writer.AcquireSpan(accessLength);
         info.AccessModifier.FillCharBuffer(in accessSpan);
      }

      var offset = accessLength > 0 ? 1 : 0;
      var modifiersSpan = builder.Writer.AcquireSpan(modifiersLength + offset);
      
      if (accessLength > 0)
      {
         modifiersSpan[0] = ' ';
      }
      
      info.ClassModifiers.FillCharBuffer(modifiersSpan[offset..]);

      var typeLength = info.Type.GetCharBufferSize();
      var nameSpan = builder.Writer.AcquireSpan(2 + info.Name.Length + typeLength);

      nameSpan[0] = ' ';
      var nameSpanSliced = nameSpan[1..];
      
      info.Type.FillCharBuffer(in nameSpanSliced);
      nameSpanSliced = nameSpanSliced[typeLength..];
      
      nameSpanSliced[0] = ' ';
      nameSpanSliced = nameSpanSliced[1..];
      info.Name.Span.CopyTo(nameSpanSliced);
      
      if (info.GenericParameterCount > 0)
      {
         builder.RenderGenericParameter(builder.RegionIndexClassGenerics);
      }

      builder.Writer.WriteLine();
      var firstBase = true;
      
      if (info.BaseClassName.Length > 0)
      {
         builder.ClassIm.FirstBaseDeclaration(info.BaseClassName.Span, false);
         firstBase = false;
      }
      
      foreach (var interfaceName in builder.GetTemporaryEnumerator<RefStringView>(builder.RegionIndexClassInterfaces))
      {
         if (firstBase)
         {
            builder.ClassIm.FirstBaseDeclaration(interfaceName.Span, false);
            firstBase = false;
         }
         else
         {
            builder.ClassIm.NextBaseDeclaration(interfaceName.Span, false);
         }
      }
      
      if (info.GenericParameterCount > 0)
      {
         builder.RenderGenericConstraints(builder.RegionIndexClassGenerics);
      }

      builder.Writer.OpenBody();
      
      builder.ClearTemporaryData(builder.RegionIndexClassInterfaces);
      builder.ClearTemporaryData(builder.RegionIndexClassGenerics);
   }
}