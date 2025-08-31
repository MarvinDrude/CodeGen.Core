using System.Runtime.CompilerServices;

namespace CodeGen.Writing.Builders.Interfaces;

public static class IMethodParameterBuilderExtensions
{
   public static ref T WriteParameterInterpolated<T>(this ref T builder, 
      bool comma,
      bool newLine,
      IFormatProvider? provider,
      [InterpolatedStringHandlerArgument(nameof(builder), nameof(provider))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, IMethodParameterBuilder, allows ref struct
   {
      ref var writer = ref builder.GetBuilder().Writer;
      
      writer.WriteInterpolated($"{(comma ? "," : string.Empty)}");
      if (newLine) writer.WriteLine();
      
      return ref builder;
   }
   
   public static ref T WriteParameterInterpolated<T>(this ref T builder,
      bool comma,
      bool newLine,
      [InterpolatedStringHandlerArgument(nameof(builder))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, IMethodParameterBuilder, allows ref struct
   {
      ref var writer = ref builder.GetBuilder().Writer;
      
      writer.WriteInterpolated($"{(comma ? "," : string.Empty)}");
      if (newLine) writer.WriteLine();
      
      return ref builder;
   }
}