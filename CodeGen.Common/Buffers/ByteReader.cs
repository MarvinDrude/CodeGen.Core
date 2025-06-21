using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Common.Buffers;

[StructLayout(LayoutKind.Sequential)]
public ref struct ByteReader
{
   private readonly ReadOnlySpan<byte> _buffer;
   private int _position;

   public ByteReader(ReadOnlySpan<byte> buffer)
   {
      _buffer = buffer;
      _position = 0;
   }

   public T ReadLittleEndian<T>()
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _buffer.Slice(_position, size);
      _position += size;

      if (BitConverter.IsLittleEndian) 
         return MemoryMarshal.Read<T>(span);
      
      Span<byte> temp = stackalloc byte[size];

      for (var e = 0; e < size; e++)
      {
         temp[e] = span[size - 1 - e];
      }
         
      return MemoryMarshal.Read<T>(temp);

   }

   public T ReadBigEndian<T>()
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _buffer.Slice(_position, size);
      _position += size;
      
      if (!BitConverter.IsLittleEndian) 
         return MemoryMarshal.Read<T>(span);
      
      Span<byte> temp = stackalloc byte[size];

      for (var e = 0; e < size; e++)
      {
         temp[e] = span[size - 1 - e];
      }
         
      return MemoryMarshal.Read<T>(temp);
   }
}