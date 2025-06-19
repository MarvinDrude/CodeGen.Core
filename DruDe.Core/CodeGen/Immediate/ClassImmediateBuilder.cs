using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.CodeGen.Immediate;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ClassImmediateBuilder : IImmediateBuilder
{
   private readonly ref byte _builderReference;
   public ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }

   public ClassImmediateBuilder(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
   }
}

public static class ClassImmediateBuilderExtensions
{
   
}