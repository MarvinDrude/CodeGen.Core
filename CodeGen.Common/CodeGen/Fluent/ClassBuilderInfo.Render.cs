using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ClassBuilderInfoExtensions
{
   public static ref FluentCodeBuilder Render(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder.Builder;

      builder.ClassIm.WriteLine(info.Name.Span);
      
      return ref info.Builder;
   }
}