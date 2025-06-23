using System.Runtime.CompilerServices;

namespace CodeGen.Common.Buffers.Dynamic;

public ref struct RegionedSpan : IDisposable
{
   private BufferWriter<byte> _totalBuffer;
   private int _regionCount;
   private int _position;

   private int _headerSize;
   private int _headerGrowSize;
   private int _headerPosition;

   private int FreeHeaderSize
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _headerSize - _headerPosition;
   }
   
   public RegionedSpan(
      Span<byte> buffer,
      int initialHeaderSize = -1)
   {
      _totalBuffer = new BufferWriter<byte>(buffer);
      _regionCount = 0;
      _position = 0;

      _headerSize = Math.Max(initialHeaderSize, DefaultHeaderSize);
      _headerGrowSize = _headerSize;
   }

   public int AddRegion<T>(int minSize)
   {
      if (FreeHeaderSize < HeaderEntrySize)
      {
         GrowHeader();
      }

      _totalBuffer.Position = _headerPosition;
      using var writer = new ByteWriter(_totalBuffer.AcquireSpan(HeaderEntrySize, false));

      writer.WriteLittleEndian(_position);
      writer.WriteLittleEndian(minSize);

      var newPosition = _position + minSize;
      _totalBuffer.AdvanceTo(newPosition);
      
      _headerPosition += HeaderEntrySize;
      _regionCount++;

      return 0;
   }

   public void Dispose()
   {
      _totalBuffer.Dispose();
   }
   
   private void GrowHeader()
   {
      var oldSize = _headerSize;
      var newSize = _headerSize + _headerGrowSize;
      
      _totalBuffer.Move(oldSize, oldSize - _totalBuffer.Capacity, newSize, false);
      _position += _headerGrowSize;
   }

   internal const int DefaultHeaderSize = 64;
   internal const int HeaderEntrySize = sizeof(int) + sizeof(int);
}