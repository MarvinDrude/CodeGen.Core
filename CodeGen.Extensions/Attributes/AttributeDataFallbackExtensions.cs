using System.Numerics;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Attributes;

public static class AttributeDataFallbackExtensions
{
   extension(AttributeData attributeData)
   {
      public string? DetermineStringValue(string name, int index, string? defaultValue = null)
      {
         return attributeData.GetNamedStringValue(name, null)
            ?? attributeData.GetStringValue(index, defaultValue);
      }

      public bool DetermineBoolValue(string name, int index, bool defaultValue = false)
      {
         return attributeData.GetNamedBoolValue(name, null)
            ?? attributeData.GetBoolValue(index, defaultValue);
      }

      public T DetermineNumberValue<T>(string name, int index, T defaultValue = default)
         where T : struct, INumber<T>
      {
         return attributeData.GetNamedNumberValue<T>(name, null)
            ?? attributeData.GetNumberValue(index, defaultValue);
      }

      public int DetermineIntValue(string name, int index, int defaultValue = 0)
         => attributeData.DetermineNumberValue(name, index, defaultValue);
      
      public long DetermineLongValue(string name, int index, long defaultValue = 0)
         => attributeData.DetermineNumberValue(name, index, defaultValue);
      
      public float DetermineFloatValue(string name, int index, float defaultValue = 0)
         => attributeData.DetermineNumberValue(name, index, defaultValue);
      
      public double DetermineDoubleValue(string name, int index, double defaultValue = 0)
         => attributeData.DetermineNumberValue(name, index, defaultValue);

      public ITypeSymbol? DetermineTypeValue(string name, int index)
      {
         return attributeData.GetNamedTypeValue(name)
               ?? attributeData.GetTypeValue(index);
      }

      public string? DetermineEnumFullNameValue(string name, int index, string? defaultValue = null)
      {
         return attributeData.GetNamedEnumFullNameValue(name, null)
            ?? attributeData.GetEnumFullNameValue(index, defaultValue);
      }

      public string?[] DetermineStringArrayValues(string name, int index, string? defaultValue = null)
      {
         return attributeData.GetNamedStringArrayValues(name, defaultValue)
            ?? attributeData.GetStringArrayValues(index, defaultValue);
      }

      public bool[] DetermineBoolArrayValues(string name, int index, bool defaultValue = false)
      {
         return attributeData.GetNamedBoolArrayValues(name, defaultValue)
            ?? attributeData.GetBoolArrayValues(index, defaultValue);
      }

      public T[] DetermineNumberArrayValues<T>(string name, int index, T defaultValue = default)
         where T : struct, INumber<T>
      {
         return attributeData.GetNamedNumberArrayValues(name, defaultValue)
            ?? attributeData.GetNumberArrayValues(index, defaultValue);
      }
      
      public int[] DetermineIntArrayValues(string name, int index, int defaultValue = 0)
         => attributeData.DetermineNumberArrayValues(name, index, defaultValue);
      
      public long[] DetermineLongArrayValues(string name, int index, long defaultValue = 0)
         => attributeData.DetermineNumberArrayValues(name, index, defaultValue);
      
      public float[] DetermineFloatArrayValues(string name, int index, float defaultValue = 0)
         => attributeData.DetermineNumberArrayValues(name, index, defaultValue);
      
      public double[] DetermineDoubleArrayValues(string name, int index, double defaultValue = 0)
         => attributeData.DetermineNumberArrayValues(name, index, defaultValue);

      public ITypeSymbol?[] DetermineTypeArrayValues(string name, int index)
      {
         return attributeData.GetNamedTypeArrayValues(name)
            ?? attributeData.GetTypeArrayValues(index);
      }

      public string?[] DetermineEnumFullNameArrayValues(string name, int index, string? defaultValue = null)
      {
         return attributeData.GetNamedEnumFullNameArrayValues(name, null)
            ?? attributeData.GetEnumFullNameArrayValues(index, defaultValue);
      }
   }
}