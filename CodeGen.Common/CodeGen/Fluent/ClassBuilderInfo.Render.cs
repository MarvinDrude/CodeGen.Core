using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ClassBuilderInfoExtensions
{
   public static ref CodeBuilder Render(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      builder.ClassIm.WriteLine(info.Name.Span);
      
      return ref info.Builder;
   }
}