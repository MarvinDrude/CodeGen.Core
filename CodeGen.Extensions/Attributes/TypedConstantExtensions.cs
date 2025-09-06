using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Extensions.Attributes;

public static class TypedConstantExtensions
{
   extension(TypedConstant typedConstant)
   {
      public string? GetStringValue(string? defaultValue = null)
      {
         return typedConstant.Value is string str 
            ? str : defaultValue;
      }

      public bool GetBoolValue(bool defaultValue = false)
      {
         return typedConstant.Value is bool boolValue 
            ? boolValue : defaultValue;
      }

      public T GetNumberValue<T>(T defaultValue = default)
         where T : struct, INumber<T>
      {
         if (typedConstant.Value is T value)
         {
            return value;
         }
         
         return defaultValue;
      }
      
      public int GetIntValue(int defaultValue = 0) 
         => typedConstant.GetNumberValue(defaultValue);
      
      public long GetLongValue(long defaultValue = 0) 
         => typedConstant.GetNumberValue(defaultValue);
      
      public float GetFloatValue(float defaultValue = 0f) 
         => typedConstant.GetNumberValue(defaultValue);
      
      public double GetDoubleValue(double defaultValue = 0d) 
         => typedConstant.GetNumberValue(defaultValue);

      public ITypeSymbol? GetTypeValue()
      {
         return typedConstant is { Kind: TypedConstantKind.Type }
            ? typedConstant.Value as ITypeSymbol : null;
      }

      public string? GetEnumFullNameValue(string? defaultValue = null)
      {
         return typedConstant is { Kind: TypedConstantKind.Enum }
            ? typedConstant.ToCSharpString() : defaultValue;
      }
   }
}