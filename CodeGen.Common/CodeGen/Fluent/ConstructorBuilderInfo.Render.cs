namespace CodeGen.Common.CodeGen.Fluent;

public static partial class ConstructorBuilderInfoExtensions
{
   internal static ref CodeBuilder RenderConstructors(this ref ClassBuilderInfo info)
   {
      ref var builder = ref info.Builder;

      var enumerator = builder.GetTemporaryEnumerator<ConstructorBuilderInfo>(builder.RegionIndexConstructors).GetEnumerator();
      while (enumerator.MoveNext())
      {
         
      }
      
      return ref builder;
   }
}