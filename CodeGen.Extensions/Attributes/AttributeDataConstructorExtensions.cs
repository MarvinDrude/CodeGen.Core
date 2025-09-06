using System.Numerics;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Attributes;

public static class AttributeDataConstructorExtensions
{
   extension(AttributeData attributeData)
   {
      public string? GetStringValue(int index, string? defaultValue = null)
      {
         return attributeData.ConstructorArguments[index].GetStringValue(defaultValue);
      }

      public bool GetBoolValue(int index, bool defaultValue = false)
      {
         return attributeData.ConstructorArguments[index].GetBoolValue(defaultValue);
      }

      public T GetNumberValue<T>(int index, T defaultValue = default)
         where T : struct, INumber<T>
      {
         return attributeData.ConstructorArguments[index].GetNumberValue(defaultValue);
      }

      public int GetIntValue(int index, int defaultValue = 0)
         => attributeData.GetNumberValue(index, defaultValue);
      
      public long GetLongValue(int index, long defaultValue = 0)
         => attributeData.GetNumberValue(index, defaultValue);
      
      public float GetFloatValue(int index, float defaultValue = 0)
         => attributeData.GetNumberValue(index, defaultValue);
      
      public double GetDoubleValue(int index, double defaultValue = 0)
         => attributeData.GetNumberValue(index, defaultValue);

      public ITypeSymbol? GetTypeValue(int index)
      {
         return attributeData.ConstructorArguments[index].GetTypeValue();
      }

      public string? GetEnumFullNameValue(int index, string? defaultValue = null)
      {
         return attributeData.ConstructorArguments[index].GetEnumFullNameValue(defaultValue);
      }

      public string?[] GetStringArrayValues(int index, string? defaultValue = null)
      {
         return attributeData.ConstructorArguments[index].GetStringArrayValues(defaultValue);
      }

      public bool[] GetBoolArrayValues(int index, bool defaultValue = false)
      {
         return attributeData.ConstructorArguments[index].GetBoolArrayValues(defaultValue);
      }

      public T[] GetNumberArrayValues<T>(int index, T defaultValue = default)
         where T : struct, INumber<T>
      {
         return attributeData.ConstructorArguments[index].GetNumberArrayValues(defaultValue);
      }

      public int[] GetIntArrayValues(int index, int defaultValue = 0)
         => attributeData.GetNumberArrayValues(index, defaultValue);
      
      public long[] GetLongArrayValues(int index, long defaultValue = 0)
         => attributeData.GetNumberArrayValues(index, defaultValue);
      
      public float[] GetFloatArrayValues(int index, float defaultValue = 0)
         => attributeData.GetNumberArrayValues(index, defaultValue);
      
      public double[] GetDoubleArrayValues(int index, double defaultValue = 0)
         => attributeData.GetNumberArrayValues(index, defaultValue);

      public ITypeSymbol?[] GetTypeArrayValues(int index)
      {
         return attributeData.ConstructorArguments[index].GetTypeArrayValues();
      }

      public string?[] GetEnumFullNameArrayValues(int index, string? defaultValue = null)
      {
         return attributeData.ConstructorArguments[index].GetEnumFullNameArrayValues(defaultValue);
      }
   }
}