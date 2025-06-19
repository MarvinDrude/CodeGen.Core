using System.Runtime.InteropServices;

namespace CodeGen.Common.CodeGen.Models.Methods;

[StructLayout(LayoutKind.Auto)]
public struct MethodParameter
{
   public string Name;
   public string Type;
}