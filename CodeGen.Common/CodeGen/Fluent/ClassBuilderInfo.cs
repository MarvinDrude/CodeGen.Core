using System.Runtime.CompilerServices;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

public ref struct ClassBuilderInfo
{
   internal RefStringView Name;
   internal RefStringView BaseClassName;

   private readonly ref byte _builderReference;
   internal ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }
   
   public ClassBuilderInfo(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
   }
}

public static partial class ClassBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo SetName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Name = name;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo SetBaseClassName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.BaseClassName = name;
      return ref info;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo AddInterfaceName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Builder.AddTemporaryData(info.Builder.RegionIndexClassInterfaces, new RefStringView(name));
      return ref info;
   }
}