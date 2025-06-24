using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ClassBuilderInfoExtensions
{
   public static ref CodeBuilder Render(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      foreach (var refInterfaceName in builder.GetTemporaryEnumerator<RefStringView>(builder.RegionIndexClassInterfaces))
      {
         _ = "";
      }
      
      return ref info.Builder;
   }
}