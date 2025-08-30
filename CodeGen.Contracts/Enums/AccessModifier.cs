namespace CodeGen.Contracts.Enums;

[Flags]
public enum AccessModifier
{
   None = 0,
   Private = 1 << 0,
   Protected = 1 << 1,
   Internal = 1 << 2,
   Public = 1 << 3,
   ProtectedInternal = Protected | Internal,
   PrivateProtected = Private | Protected
}

public static class AccessModifierExtensions
{
   extension(AccessModifier modifier)
   {
      public void FillCharBuffer(scoped in Span<char> buffer)
      {
         ReadOnlySpan<char> target = modifier switch
         {
            AccessModifier.None => string.Empty,
            AccessModifier.PrivateProtected => PrivateProtected,
            AccessModifier.ProtectedInternal => ProtectedInternal,
            AccessModifier.Private => Private,
            AccessModifier.Protected => Protected,
            AccessModifier.Internal => Internal,
            AccessModifier.Public => Public,
            _ => string.Empty
         };
         
         target.CopyTo(buffer);
      }
      
      public int GetCharBufferSize()
      {
         return modifier switch
         {
            AccessModifier.None => 0,
            AccessModifier.Private => Private.Length,
            AccessModifier.Protected => Protected.Length,
            AccessModifier.PrivateProtected => PrivateProtected.Length,
            AccessModifier.Internal => Internal.Length,
            AccessModifier.ProtectedInternal => ProtectedInternal.Length,
            AccessModifier.Public => Public.Length,
            _ => 0
         };
      }
   }
   
   private const string PrivateProtected = "private protected";
   private const string ProtectedInternal = "protected internal";
   private const string Private = "private";
   private const string Protected = "protected";
   private const string Internal = "internal";
   private const string Public = "public";
}