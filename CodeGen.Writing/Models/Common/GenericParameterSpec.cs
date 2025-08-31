using System.Runtime.InteropServices;
using CodeGen.Contracts.Enums;

namespace CodeGen.Writing.Models.Common;

[StructLayout(LayoutKind.Auto)]
public readonly record struct GenericParameterSpec(
   string Name,
   GenericType Variance);