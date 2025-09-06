using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Formats;

public static class SymbolDisplayFormats
{
   public static readonly SymbolDisplayFormat FullyQualifiedNoGenerics =
      new (
         globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
         typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
         genericsOptions: SymbolDisplayGenericsOptions.None,
         miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable
      );
}