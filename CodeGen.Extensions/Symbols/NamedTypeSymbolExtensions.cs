using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Symbols;

public static class NamedTypeSymbolExtensions
{
   extension(INamedTypeSymbol symbol)
   {
      public bool IsVoidTask => symbol is
      {
         Name: "Task",
         ContainingNamespace:
         {
            Name: "Tasks",
            ContainingNamespace:
            {
               Name: "Threading",
               ContainingNamespace:
               {
                  Name: "System",
                  ContainingNamespace.IsGlobalNamespace: true
               }
            }
         }
      };

      public string TypeAsString => symbol switch
      {
         { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
         { IsRecord: true } => "record",
         { TypeKind: TypeKind.Interface } => "interface",
         { TypeKind: TypeKind.Struct } => "struct",
         { TypeKind: TypeKind.Enum } => "enum",
         { TypeKind: TypeKind.Delegate } => "delegate",
         
         _ => "class"
      };
   }
}