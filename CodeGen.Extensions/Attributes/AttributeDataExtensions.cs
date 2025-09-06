using System.Numerics;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Attributes;

public static class AttributeDataExtensions
{
   extension(AttributeData attributeData)
   {
      public bool TryGetNamedArgument(string name, out TypedConstant value)
      {
         foreach (var arg in attributeData.NamedArguments)
         {
            if (arg.Key != name) continue;
            
            value = arg.Value;
            return true;
         }
         
         value = default;
         return false;
      }

      public string? GetNamedStringValue(string name, string? defaultValue = null)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringValue(defaultValue)
            : defaultValue;
      }

      public bool? GetNamedBoolValue(string name, bool? defaultValue = null)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolValue(defaultValue ?? false)
            : defaultValue;
      }

      public T? GetNamedNumberValue<T>(string name, T? defaultValue = null)
         where T : struct, INumber<T>
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberValue(defaultValue ?? default)
            : defaultValue;
      }
      
      public int? GetNamedIntValue(string name, int? defaultValue = null)
         => attributeData.GetNamedNumberValue(name, defaultValue);
      
      public long? GetNamedLongValue(string name, long? defaultValue = null)
         => attributeData.GetNamedNumberValue(name, defaultValue);
      
      public float? GetNamedFloatValue(string name, float? defaultValue = null)
         => attributeData.GetNamedNumberValue(name, defaultValue);
      
      public double? GetNamedDoubleValue(string name, double? defaultValue = null)
         => attributeData.GetNamedNumberValue(name, defaultValue);

      public ITypeSymbol? GetNamedTypeValue(string name)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeValue()
            : null;
      }

      public string? GetEnumFullNameValue(string name, string? defaultValue = null)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameValue(defaultValue)
            : defaultValue;
      }

      public string?[]? GetNamedStringArrayValues(string name, string? defaultValue = null)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetStringArrayValues(defaultValue)
            : null;
      }

      public bool[]? GetNamedBoolArrayValues(string name, bool defaultValue = false)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetBoolArrayValues(defaultValue)
            : null;
      }

      public T[]? GetNamedNumberArrayValues<T>(string name, T defaultValue = default)
         where T : struct, INumber<T>
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetNumberArrayValues(defaultValue)
            : null;
      }
      
      public int[]? GetNamedIntArrayValues(string name, int defaultValue = 0)
         => attributeData.GetNamedNumberArrayValues(name, defaultValue);
      
      public long[]? GetNamedLongArrayValues(string name, long defaultValue = 0)
         => attributeData.GetNamedNumberArrayValues(name, defaultValue);
      
      public float[]? GetNamedFloatArrayValues(string name, float defaultValue = 0)
         => attributeData.GetNamedNumberArrayValues(name, defaultValue);
      
      public double[]? GetNamedDoubleArrayValues(string name, double defaultValue = 0)
         => attributeData.GetNamedNumberArrayValues(name, defaultValue);

      public ITypeSymbol?[]? GetNamedTypeArrayValues(string name)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetTypeArrayValues()
            : null;
      }

      public string?[]? GetNamedEnumFullNameArrayValues(string name, string? defaultValue = null)
      {
         return attributeData.TryGetNamedArgument(name, out var constant)
            ? constant.GetEnumFullNameArrayValues(defaultValue)
            : null;
      }
   }
}