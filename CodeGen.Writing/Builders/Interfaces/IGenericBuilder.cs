using CodeGen.Contracts.Enums;

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
      
      public ref T WriteEndGenericParameters()
      {
         ref var writer = ref builder.GetBuilder().Writer;
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
   }
}