using CodeGen.Extensions.Formats;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Symbols;

public static class SymbolLikeExtensions
{
   extension<T>(T symbol)
      where T : ISymbol
   {
      public bool IsVoidTask => symbol 
         is INamedTypeSymbol { IsVoidTask: true };

      public string ToMetadataFullyQualifiedName()
      {
         var origin = symbol.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNoGenerics);

         if (symbol is INamedTypeSymbol { IsGenericType: true } named)
         {
            origin += $"`{named.TypeArguments.Length}";
         }
         
         return origin;
      }
   }
}