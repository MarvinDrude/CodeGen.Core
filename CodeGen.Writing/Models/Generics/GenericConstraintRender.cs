using System.Runtime.InteropServices;

namespace CodeGen.Writing.Models.Generics;

[StructLayout(LayoutKind.Auto)]
public struct GenericConstraintRender
{
   public string? Expression;
}