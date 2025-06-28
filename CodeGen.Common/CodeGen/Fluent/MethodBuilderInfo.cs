using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct MethodBuilderInfo
{
   internal AccessModifier AccessModifier;
   internal MethodModifier Modifiers;
   
   internal RefStringView Name;
   internal RefStringView ReturnType;

   internal bool IsSignatureOnly;
   
   internal int ParameterOffset;
   internal int ParameterCount;
   internal int ParameterLength;
   
   internal int GenericParameterCount;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public MethodBuilderInfo(ref ClassBuilderInfo builder, ReadOnlySpan<char> name)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
      Name = name;
   }
   
   
}