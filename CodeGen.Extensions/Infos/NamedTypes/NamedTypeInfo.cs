using CodeGen.Contracts.Enums;

namespace CodeGen.Extensions.Infos.NamedTypes;

public readonly record struct NamedTypeInfo<TModifiers>(
   string Name,
   string? NameSpace,
   AccessModifier Access,
   TModifiers Modifiers,
   string FullPathName,
   string FullPathMetadataName)
{
   public string FileNameSpace => NameSpace ?? "Global";
   
   public string GeneratedFileName => $"{FileNameSpace}.{Name}.g.cs";
}