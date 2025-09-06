using System.Numerics;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Attributes;

public static class TypedConstantArrayExtensions
{
   extension(TypedConstant typedConstant)
   {
      public bool IsArray => typedConstant is { Kind: TypedConstantKind.Array };
      
      public string?[] GetStringArrayValues(string? defaultValue = null)
      {
         return typedConstant.IsArray
            ? typedConstant.Values.Select(x => x.GetStringValue(defaultValue)).ToArray()
            : [];
      }

      public bool[] GetBoolArrayValues(bool defaultValue = false)
      {
         return typedConstant.IsArray
            ? typedConstant.Values.Select(x => x.GetBoolValue(defaultValue)).ToArray()
            : [];
      }

      public T[] GetNumberArrayValues<T>(T defaultValue = default)
         where T : struct, INumber<T>
      {
         return typedConstant.IsArray
            ? typedConstant.Values.Select(x => x.GetNumberValue(defaultValue)).ToArray()
            : [];
      }
      
      public int[] GetIntArrayValues(int defaultValue = 0)
         => typedConstant.GetNumberArrayValues(defaultValue);
      
      public long[] GetLongArrayValues(long defaultValue = 0)
         => typedConstant.GetNumberArrayValues(defaultValue);
      
      public float[] GetFloatArrayValues(float defaultValue = 0)
         => typedConstant.GetNumberArrayValues(defaultValue);
      
      public double[] GetDoubleArrayValues(double defaultValue = 0)
         => typedConstant.GetNumberArrayValues(defaultValue);

      public ITypeSymbol?[] GetTypeArrayValues()
      {
         return typedConstant.IsArray
            ? typedConstant.Values.Select(x => x.GetTypeValue()).ToArray()
            : [];
      }

      public string?[] GetEnumFullNameArrayValues()
      {
         return typedConstant.IsArray
            ? typedConstant.Values.Select(x => x.GetEnumFullNameValue()).ToArray()
            : [];
      }
   }
}