using CodeGen.Contracts.Enums;
using CodeGen.Extensions.Infos.NamedTypes;
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
      
      public bool IsStruct => symbol.TypeKind == TypeKind.Struct;
      public bool IsEnum => symbol.TypeKind == TypeKind.Enum;
      public bool IsDelegate => symbol.TypeKind == TypeKind.Delegate;
      public bool IsInterface => symbol.TypeKind == TypeKind.Interface;
      public bool IsRecord => symbol.IsRecord;
      public bool IsClass => symbol.TypeKind == TypeKind.Class;

      public NamedTypeInfo<ClassModifier> CreateClassInfo()
      {
         return new NamedTypeInfo<ClassModifier>();
      }

      public NamedTypeInfo<InterfaceModifier> CreateInterfaceInfo()
      {
         return new NamedTypeInfo<InterfaceModifier>();
      }

      public NamedTypeInfo<StructModifier> CreateStructInfo()
      {
         return new NamedTypeInfo<StructModifier>();
      }
   }
}