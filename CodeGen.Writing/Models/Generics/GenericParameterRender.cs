using System.Runtime.InteropServices;
using CodeGen.Contracts.Enums;

namespace CodeGen.Writing.Models.Generics;

[StructLayout(LayoutKind.Auto)]
public struct GenericParameterRender
{
   public string? Name;
   public GenericType Type;

   public int ConstraintIndex;
   public int ConstraintCount;
}