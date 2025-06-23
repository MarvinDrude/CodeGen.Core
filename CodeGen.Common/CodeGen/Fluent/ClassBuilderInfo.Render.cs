using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ClassBuilderInfoExtensions
{
   public static ref CodeBuilder Render(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      
      
      return ref info.Builder;
   }
}