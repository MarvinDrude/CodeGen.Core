using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ParameterBuilderInfo
{
   internal RefStringView TypeName;
   internal RefStringView Name;

   internal RefStringView Attributes;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public ParameterBuilderInfo(ref ClassBuilderInfo builder)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
   }
   
   
}

public static partial class ParameterBuilderInfoExtensions
{
   
}