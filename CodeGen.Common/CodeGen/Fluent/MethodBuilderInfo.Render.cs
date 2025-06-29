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
      
   }
}