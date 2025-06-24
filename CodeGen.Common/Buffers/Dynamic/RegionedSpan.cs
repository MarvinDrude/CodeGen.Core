using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeGen.Common.Buffers.Dynamic;

[DebuggerTypeProxy(typeof(RegionedSpanDebugView))]
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

   public RegionEnumeratorHolder<T> GetRegionEnumerator<T>(int regionIndex)
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      if (regionIndex < 0 || regionIndex >= _regionCount)
      {
         throw new ArgumentOutOfRangeException(nameof(regionIndex));
      }

      var region = GetRegionMetadata(regionIndex);
      return new RegionEnumeratorHolder<T>(_totalBuffer.Owner.Span, region, regionIndex);
   }
   
   public void AddToRegion<T>(int regionIndex, scoped in T data)
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      if (regionIndex < 0 || regionIndex >= _regionCount)
      {
         throw new ArgumentOutOfRangeException(nameof(regionIndex));
      }

      var region = GetRegionMetadata(regionIndex);
      var dataSize = T.CalculateByteLength(in data);
      var newTotalSize = region.InnerOffset + dataSize;

      if (newTotalSize > region.ByteLength)
      {
         ResizeRegion(regionIndex, newTotalSize);
      }
      
      var headerOffset = HeaderEntrySize * regionIndex;
      _totalBuffer.Position = headerOffset;
      
      var writer = new ByteWriter(_totalBuffer.Owner.Span);
      writer.Position = headerOffset + sizeof(int) + sizeof(int);
      writer.WriteLittleEndian(newTotalSize);
      writer.WriteLittleEndian(region.ItemCount + 1);

      T.Write(_totalBuffer.Owner.Span.Slice(
         region.Offset + region.InnerOffset, dataSize), in data);
   }

   public void ClearRegion(int regionIndex, int setSize = -1)
   {
      if (regionIndex < 0 || regionIndex >= _regionCount)
      {
         throw new ArgumentOutOfRangeException(nameof(regionIndex));
      }

      var region = GetRegionMetadata(regionIndex);
      
      if (region.ItemCount > 0)
      {
         var headerOffset = HeaderEntrySize * regionIndex;
         _totalBuffer.Position = 0;
      
         var writer = new ByteWriter(_totalBuffer.Owner.Span);
         writer.Position = headerOffset + sizeof(int) + sizeof(int);

         writer.WriteLittleEndian(0);
         writer.WriteLittleEndian(0);
      }

      if (setSize >= 0)
      {
         ResizeRegion(regionIndex, setSize);
      }
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
      var regionInnerPosition = reader.ReadLittleEndian<int>();

      ArgumentOutOfRangeException.ThrowIfLessThan(newSize, regionInnerPosition);

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
      writer.WriteLittleEndian(0);
      writer.WriteLittleEndian(0);

      var newPosition = _position + minSize;
      _totalBuffer.AdvanceTo(newPosition);
      
      _headerPosition += HeaderEntrySize;
      _position = newPosition;

      return _regionCount++;
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
   
   internal SpanRegion GetRegionMetadata(int regionIndex)
   {
      var headerOffset = HeaderEntrySize * regionIndex;
      _totalBuffer.Position = headerOffset;
      
      var reader = new ByteReader(_totalBuffer.AcquireSpan(HeaderEntrySize, false));

      return new SpanRegion(
         reader.ReadLittleEndian<int>(),
         reader.ReadLittleEndian<int>(),
         reader.ReadLittleEndian<int>(),
         reader.ReadLittleEndian<int>());
   }

   internal void SetRegionMetadata(int regionIndex, SpanRegion region)
   {
      var headerOffset = HeaderEntrySize * regionIndex;
      _totalBuffer.Position = headerOffset;
      
      var writer = new ByteWriter(_totalBuffer.AcquireSpan(HeaderEntrySize, false));

      writer.WriteLittleEndian(region.Offset);
      writer.WriteLittleEndian(region.ByteLength);
      writer.WriteLittleEndian(region.InnerOffset);
      writer.WriteLittleEndian(region.ItemCount);
      
      writer.Dispose();
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
   internal const int HeaderEntrySize = sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int);

   public readonly ref struct RegionEnumeratorHolder<T>
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      private readonly Span<byte> _span;
      private readonly SpanRegion _region;
      private readonly int _regionIndex;
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public RegionEnumeratorHolder(
         Span<byte> span, SpanRegion region, int regionIndex)
      {
         _span = span;
         _regionIndex = regionIndex;
         _region = region;
      }
      
      public RegionEnumerator<T> GetEnumerator() => new (_span, _region, _regionIndex);
   }
   
   public ref struct RegionEnumerator<T>
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      private readonly Span<byte> _span;
      private readonly SpanRegion _region;
      private readonly int _regionIndex;
      private int _index;
      private int _offset;

      private T _current;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal RegionEnumerator(
         Span<byte> span, SpanRegion region, int regionIndex)
      {
         _span = span;
         _regionIndex = regionIndex;
         _region = region;
         _index = -1;
         _offset = 0;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool MoveNext()
      {
         var index = _index + 1;
         if (index >= _region.ItemCount)
         {
            return false;
         }

         _index = index;
         var read = T.Read(_span[(_region.Offset + _offset)..], out var current);
         _current = current;
         
         _offset += read;
         return true;
      }

      public T Current
      {
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         get => _current;
      }
   }
   
   public ref struct RegionedSpanDebugView
   {
      public List<ManagedSpanRegion> RegionMetadata { get; } 
      
      private readonly RegionedSpan _source;
   
      public RegionedSpanDebugView(RegionedSpan source)
      {
         _source = source;

         RegionMetadata = [];
         for (var e = 0; e < RegionCount; e++)
         {
            RegionMetadata.Add(new ManagedSpanRegion(source.GetRegionMetadata(e)));
         }
      }

      public int RegionCount => _source._regionCount;

      public sealed class ManagedSpanRegion
      {
         public int Offset { get; set; }
         public int ByteLength { get; set; }
         public int InnerOffset { get; set; }
         public int ItemCount { get; set; }
         
         public ManagedSpanRegion(SpanRegion region)
         {
            Offset = region.Offset;
            ByteLength = region.ByteLength;
            InnerOffset = region.InnerOffset;
            ItemCount = region.ItemCount;
         }
      }
   }
}
