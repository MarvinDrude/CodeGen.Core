using System.Runtime.CompilerServices;

namespace CodeGen.Common.Buffers.Dynamic;

public ref struct RegionedSpan : IDisposable
{
   private BufferWriter<byte> _totalBuffer;
   private int _regionCount;
   private int _position;

   private int _headerSize;
   private readonly int _headerGrowSize;
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

      _headerSize = Math.Max(initialHeaderSize, DefaultHeaderSize);
      _headerGrowSize = _headerSize;
      _position = _headerSize;
   }

   public void ResizeRegion(int regionIndex, int newSize)
   {
      if (regionIndex < 0 || regionIndex >= _regionCount)
      {
         throw new ArgumentOutOfRangeException(nameof(regionIndex));
      }
      
      var headerOffset = HeaderEntrySize * regionIndex;
      _totalBuffer.Position = 0;

      var headerSpan = _totalBuffer.AcquireSpan(_headerSize);
      
      var reader = new ByteReader(headerSpan)
      {
         Position = headerOffset
      };

      var position = reader.ReadLittleEndian<int>();
      var size = reader.ReadLittleEndian<int>();

      _totalBuffer.Position = 0;
      var writer = new ByteWriter(headerSpan);
      
      writer.Position = headerOffset + sizeof(int);
      writer.WriteLittleEndian(newSize);
      
      var afterOldOffset = position + size;
      var afterNewOffset = position + newSize;

      var afterOldLength = _position - afterOldOffset;
      
      var differenceSize = newSize - size;
      _totalBuffer.Move(afterOldOffset, afterOldLength, afterNewOffset, false);
      _position += differenceSize;

      for (var e = regionIndex + 1; e < _regionCount; e++)
      {
         writer.Position = reader.Position = HeaderEntrySize * e;
         writer.WriteLittleEndian(reader.ReadLittleEndian<int>() + differenceSize);
      }
      
      writer.Dispose(); // header writer dispose not necessary, since it will never grow
   }
   
   public Span<byte> GetRegion(int regionIndex)
   {
      if (regionIndex < 0 || regionIndex >= _regionCount)
      {
         throw new ArgumentOutOfRangeException(nameof(regionIndex));
      }

      var headerOffset = HeaderEntrySize * regionIndex;
      _totalBuffer.Position = headerOffset;
      
      var reader = new ByteReader(_totalBuffer.AcquireSpan(HeaderEntrySize, false));
      var position = reader.ReadLittleEndian<int>();
      var size = reader.ReadLittleEndian<int>();

      _totalBuffer.Position = position;
      return _totalBuffer.AcquireSpan(size, false);
   }

   public int AddRegion(int minSize)
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
      _position = newPosition;

      return _regionCount++;
   }

   public void Dispose()
   {
      _totalBuffer.Dispose();
   }
   
   private void GrowHeader()
   {
      var oldSize = _headerSize;
      var newSize = _headerSize + _headerGrowSize;
      
      _totalBuffer.Move(oldSize, _totalBuffer.Capacity - oldSize, newSize, false);
      _position += _headerGrowSize;

      _headerSize = newSize;
   }

   internal const int DefaultHeaderSize = 64;
   internal const int HeaderEntrySize = sizeof(int) + sizeof(int);
}