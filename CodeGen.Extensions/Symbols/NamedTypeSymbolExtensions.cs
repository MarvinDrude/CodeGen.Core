using CodeGen.Contracts.Enums;
using CodeGen.Extensions.Enums;
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
         var modifiers = ClassModifier.None;
         if (symbol.IsStatic) modifiers |= ClassModifier.Static;
         if (symbol.IsAbstract) modifiers |= ClassModifier.Abstract;
         if (symbol.IsSealed) modifiers |= ClassModifier.Sealed;
         if (symbol.IsRecord) modifiers |= ClassModifier.Record;
         if (symbol.DeclaringSyntaxReferences.Length > 1) modifiers |= ClassModifier.Partial;
         
         return symbol.CreateTypeInfo(modifiers);
      }

      public NamedTypeInfo<InterfaceModifier> CreateInterfaceInfo()
      {
         var modifiers = InterfaceModifier.None;
         if (symbol.DeclaringSyntaxReferences.Length > 1) modifiers |= InterfaceModifier.Partial;
         
         return symbol.CreateTypeInfo(modifiers);
      }

      public NamedTypeInfo<StructModifier> CreateStructInfo()
      {
         var modifiers = StructModifier.None;
         if (symbol.DeclaringSyntaxReferences.Length > 1) modifiers |= StructModifier.Partial;
         if (symbol.IsReadOnly) modifiers |= StructModifier.ReadOnly;
         if (symbol.IsRefLikeType) modifiers |= StructModifier.Ref;
         if (symbol.IsRecord) modifiers |= StructModifier.Record;
         
         return symbol.CreateTypeInfo(modifiers);
      }

      private NamedTypeInfo<T> CreateTypeInfo<T>(T modifiers) 
         where T : Enum
      {
         var nameSpace = symbol.ContainingNamespace.ToString();
         if (nameSpace == "<global namespace>")
         {
            nameSpace = null;
         }

         return new NamedTypeInfo<T>(
            symbol.Name,
            nameSpace,
            symbol.DeclaredAccessibility.ToModifier(),
            modifiers,
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            symbol.ToMetadataFullyQualifiedName());
      }
   }
}