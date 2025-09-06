using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Symbols;

public static class SymbolLikeExtensions
{
   extension<T>(T symbol)
      where T : ISymbol
   {
      public bool IsVoidTask => symbol 
         is INamedTypeSymbol { IsVoidTask: true };
   }
}