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
   }
}