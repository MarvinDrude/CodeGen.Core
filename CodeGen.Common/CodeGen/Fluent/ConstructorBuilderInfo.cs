using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ConstructorBuilderInfo : IByteSerializable<ConstructorBuilderInfo>
{
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public ConstructorBuilderInfo(ref ClassBuilderInfo builder)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref ClassBuilderInfo Done()
   {
      ref var builder = ref ClassBuilder.Builder;
      
      builder.AddTemporaryData(builder.RegionIndexConstructors, in this);
      
      return ref ClassBuilder;
   }
}

public static partial class ConstructorBuilderInfoExtensions
{
   
}