using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Common.Buffers.Dynamic;

[StructLayout(LayoutKind.Sequential)]
public ref struct PackedBools
{
   public byte RawByte
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _flags;
   }
   private byte _flags;

   public PackedBools(byte flags)
   {
      _flags = flags;
   }
   
   public void Set(int index, bool value)
   {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 7, nameof(index));

      if (value)
      {
         _flags |= (byte)(1 << index);
         return;
      }
      
      _flags &= (byte)~(1 << index);
   }

   public bool Get(int index)
   {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 7, nameof(index));
      
      return (_flags & (1 << index)) != 0;
   }
}