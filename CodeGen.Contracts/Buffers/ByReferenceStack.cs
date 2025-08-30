using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Contracts.Buffers;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ByReferenceStack
{
   public readonly ref byte Value;
   
   public ByReferenceStack(ref byte value)
   {
      Value = ref value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref T AsRef<T>()
      where T : allows ref struct
   {
      return ref Unsafe.As<byte, T>(ref Value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T AsValue<T>()
      where T : allows ref struct
   {
      return Unsafe.As<byte, T>(ref Value);
   }
   
   public static ByReferenceStack Create<T>(ref T reference)
      where T : allows ref struct
   {
      return new ByReferenceStack(ref Unsafe.As<T, byte>(ref reference));
   }
}