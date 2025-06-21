using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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

   public int WriteBigEndian<T>(T value)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _writer.AcquireSpan(size);

      MemoryMarshal.Write(span, in value);
      
      if (BitConverter.IsLittleEndian)
      {
         span.Reverse();
      }

      return size;
   }

   public int WriteLittleEndian<T>(T value)
      where T : unmanaged
   {
      var size = Unsafe.SizeOf<T>();
      var span = _writer.AcquireSpan(size);
      
      MemoryMarshal.Write(span, in value);
      
      if (!BitConverter.IsLittleEndian)
      {
         span.Reverse();
      }

      return size;
   }

   /// <summary>
   /// Only meant for intra process / memory to memory since it's just a recast.
   /// Same endianness required too.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteStringRaw(scoped ReadOnlySpan<char> text)
   {
      var rawBytes = MemoryMarshal.AsBytes(text);
      _writer.Write(rawBytes);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int WriteString(scoped ReadOnlySpan<char> text)
   {
      return WriteString(text, Encoding.UTF8);
   }
   
   public int WriteString(scoped ReadOnlySpan<char> text, Encoding encoding)
   {
      var size = encoding.GetByteCount(text);
      var span = _writer.AcquireSpan(size);

      var written = encoding.GetBytes(text, span);
      ArgumentOutOfRangeException.ThrowIfNotEqual(size, written);
      
      return written;
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