using System.Runtime.CompilerServices;

namespace CodeGen.Common.Buffers.Dynamic;

public ref struct RegionedSpan
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
         ResizeHeader();
      }
      
      _regionCount++;

      return 0;
   }

   private void ResizeHeader()
   {
   }

   private const int DefaultHeaderSize = 64;
   private const int HeaderEntrySize = sizeof(int) + sizeof(int);
}