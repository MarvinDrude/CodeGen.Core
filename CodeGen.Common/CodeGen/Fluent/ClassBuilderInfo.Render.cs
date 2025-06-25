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
      
      foreach (var interfaceName in builder.GetTemporaryEnumerator<RefStringView>(builder.RegionIndexClassInterfaces))
      {
         _ = "";
      }
      
      return ref info.Builder;
   }

   internal static void RenderHeader(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      var accessLength = info.AccessModifier.GetCharBufferSize();
      var modifiersLength = info.ClassModifiers.GetCharBufferSize();
      
   }
}