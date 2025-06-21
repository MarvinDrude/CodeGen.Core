using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Common.Buffers;

[StructLayout(LayoutKind.Sequential)]
public ref struct ByteWriter : IDisposable
{
   public ReadOnlySpan<byte> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _writer.WrittenSpan;
   }
   
   private BufferWriter<byte> _writer;
   
   public ByteWriter(
      Span<byte> buffer,
      int initialMinGrowCapacity = 512)
   {
      _writer = new BufferWriter<byte>(buffer, initialMinGrowCapacity);   
   }

   public void WriteBigEndian<T>(T value)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _writer.AcquireSpan(size);

      MemoryMarshal.Write(span, in value);
      
      if (BitConverter.IsLittleEndian)
      {
         span.Reverse();
      }
   }

   public void WriteLittleEndian<T>(T value)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _writer.AcquireSpan(size);
      
      MemoryMarshal.Write(span, in value);
      
      if (!BitConverter.IsLittleEndian)
      {
         span.Reverse();
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteBytes(ReadOnlySpan<byte> buffer)
   {
      _writer.Write(buffer);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteBytes(Span<byte> buffer)
   {
      _writer.Write(buffer);
   }

   public void Dispose()
   {
      _writer.Dispose();
   }
}