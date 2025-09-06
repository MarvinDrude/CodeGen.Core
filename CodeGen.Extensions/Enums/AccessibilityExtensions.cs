using CodeGen.Contracts.Enums;
using Microsoft.CodeAnalysis;

namespace CodeGen.Extensions.Enums;

public static class AccessibilityExtensions
{
   extension(Accessibility access)
   {
      public AccessModifier ToModifier() => access switch
      {
         Accessibility.Public => AccessModifier.Public,
         Accessibility.Internal => AccessModifier.Internal,
         Accessibility.Private => AccessModifier.Private,
         Accessibility.Protected => AccessModifier.Protected,
         Accessibility.ProtectedAndInternal => AccessModifier.PrivateProtected,
         Accessibility.ProtectedOrInternal => AccessModifier.ProtectedInternal,
         _ => AccessModifier.None
      };
   }
}