using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Symbols;

public static class SymbolExtensions
{
   extension(ISymbol symbol)
   {
      public List<ISymbol> ExplicitOrImplicitInterfaceImplementations()
      {
         if (symbol.Kind != SymbolKind.Method &&
             symbol.Kind != SymbolKind.Property &&
             symbol.Kind != SymbolKind.Event)
         {
            return [];
         }
      
         var containingType = symbol.ContainingType;

         return containingType
            .AllInterfaces
            .SelectMany(iface => iface.GetMembers())
            .Where(interfaceMember =>
               SymbolEqualityComparer.Default.Equals(
                  symbol, 
                  containingType.FindImplementationForInterfaceMember(interfaceMember)))
            .ToList();
      }
   }
}