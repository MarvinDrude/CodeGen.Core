using CodeGen.Contracts.Enums;
using CodeGen.Writing.Models.Common;

namespace CodeGen.Writing.Builders.Interfaces;

public interface IGenericBuilder : ICodeBuilder;

public static class GenericBuilderExtensions
{
   extension<T>(ref T builder)
      where T : struct, IGenericBuilder, allows ref struct
   {
      public ref T WriteStartGenericParameters()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.Write("<");
         
         return ref builder;
      }
      
      public ref T WriteEndGenericParameters(bool newLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;

         if (newLine)
            writer.WriteLine(">");
         else
            writer.Write(">");
         
         return ref builder;
      }

      public ref T WriteGenericParameter(
         string name,
         bool addSpaceCommaInFront = false,
         GenericType type = GenericType.Invariant)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         var variance = type switch
         {
            GenericType.Invariant => string.Empty,
            GenericType.Covariant => "out ",
            GenericType.Contravariant => "in ",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
         };
         
         writer.WriteInterpolated($"{(addSpaceCommaInFront ? ", " : string.Empty)}{variance}{name}");
         
         return ref builder;
      }

      public ref T WriteGenericParameters(scoped in ReadOnlySpan<GenericParameterSpec> parameters)
      {
         builder.WriteStartGenericParameters();
         
         for (var index = 0; index < parameters.Length; index++)
         {
            scoped ref readonly var parameter = ref parameters[index];
            builder.WriteGenericParameter(parameter.Name, index != 0, parameter.Variance);
         }

         builder.WriteEndGenericParameters();
         return ref builder;
      }

      public ref T WriteStartGenericConstraints(string parameterName)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         
         writer.UpIndent();
         writer.WriteInterpolated($"where {parameterName} : ");
         
         return ref builder;
      }
      
      public ref T WriteLineEndGenericConstraints()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         
         writer.DownIndent();
         writer.WriteLine();

         return ref builder;
      }

      public ref T WriteGenericConstraint(string constraint, bool addSpaceCommaInFront = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         
         writer.WriteInterpolated($"{(addSpaceCommaInFront ? ", " : string.Empty)}{constraint}");
         
         return ref builder;
      }
   }
}