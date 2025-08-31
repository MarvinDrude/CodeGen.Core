namespace CodeGen.Writing.Builders.Interfaces;

public interface IMethodParameterBuilder : ICodeBuilder;

public static class MethodParameterBuilderExtensions
{
   extension<T>(ref T builder)
      where T : struct, IMethodParameterBuilder, allows ref struct
   {
      public ref T WriteStartParameterList(bool startParametersNewLine = true)
      {
         ref var writer = ref builder.GetBuilder().Writer;

         if (startParametersNewLine)
         {
            writer.WriteLine("(");
            writer.UpIndent();
         }
         else
            writer.Write("(");
         
         return ref builder;
      }

      public ref T WriteEndParameterList(bool downIndent = true)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.Write(")");
         
         if (downIndent)
         {
            writer.WriteLine();
            writer.DownIndent();
         }
         
         return ref builder;
      }

      public ref T WriteParameter(string expression, bool comma = true, bool newLine = true)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         
         writer.WriteInterpolated($"{expression}{(comma ? "," : string.Empty)}");
         if (newLine) writer.WriteLine();
         
         return ref builder;
      }

      public ref T WriteLastParameter(string expression)
      {
         return ref builder.WriteParameter(expression, false, false);
      }
   }
}