namespace CodeGen.Contracts.Enums;

public enum GenericType : byte
{
   /// <summary>
   /// Invariant
   /// </summary>
   Invariant = 0,
   /// <summary>
   /// In
   /// </summary>
   Contravariant = 1,
   /// <summary>
   /// Out
   /// </summary>
   Covariant = 2,
}