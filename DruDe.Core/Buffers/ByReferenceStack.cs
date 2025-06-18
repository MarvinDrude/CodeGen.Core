using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.Buffers;

/// <summary>
/// Use this only if you really know what you want to do with it.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly ref struct ByReferenceStack<T>
   where T : allows ref struct
{
   public readonly ref byte Value;
   
   public ByReferenceStack(ref byte value)
   {
      Value = ref value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref T AsRef()
   {
      return ref Unsafe.As<byte, T>(ref Value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T AsValue()
   {
      return Unsafe.As<byte, T>(ref Value);
   }
   
   public static ByReferenceStack<T> Create(ref T reference)
   {
      return new ByReferenceStack<T>(ref Unsafe.As<T, byte>(ref reference));
   }
}