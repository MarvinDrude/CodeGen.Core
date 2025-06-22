using System.Runtime.CompilerServices;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

public ref struct ClassBuilderInfo
{
   internal RefStringView Name;

   private readonly ref byte _builderReference;
   internal ref FluentCodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, FluentCodeBuilder>(ref _builderReference);
   }
   
   public ClassBuilderInfo(ref FluentCodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<FluentCodeBuilder, byte>(ref builder);
   }
}

public static partial class ClassBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo WithName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Name = name;
      return ref info;
   }
}