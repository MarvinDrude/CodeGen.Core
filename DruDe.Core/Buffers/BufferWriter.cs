using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.Buffers;

[StructLayout(LayoutKind.Auto)]
public ref struct BufferWriter<T> : IDisposable
{
   public readonly int Capacity => _owner.Length;
   public readonly int FreeCapacity => _owner.Length - _position;

   public readonly ReadOnlySpan<T> WrittenSpan => Buffer[.._position];
   public readonly BufferOwner<T> Owner => _owner;
   
   public int Position
   {
      readonly get => _position;
      set
      {
         if (value > Capacity || value < 0)
         {
            throw new ArgumentOutOfRangeException(nameof(Position));
         }
         _position = value;
      }
   }
   
   private readonly Span<T> Buffer => _owner.Span;
   private BufferOwner<T> _owner;
   private bool _isGrown;
   private bool _disposed;

   private readonly int _initialMinGrowCapacity;
   private int _position;
   
   public BufferWriter(Span<T> startBuffer, int initialMinGrowCapacity = -1)
   {
      _owner = BufferAllocator<T>.Create(startBuffer);
      _initialMinGrowCapacity = initialMinGrowCapacity;
      
      _isGrown = false;
      _disposed = false;
      _position = 0;
   }

   public BufferWriter(BufferOwner<T> owner, int initialMinGrowCapacity = -1)
   {
      _owner = owner;
      _initialMinGrowCapacity = initialMinGrowCapacity;
      
      _isGrown = false;
      _disposed = false;
      _position = 0;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add(ref T reference)
   {
      Add() = reference;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add(T value)
   {
      Add() = value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private ref T Add()
   {
      ref var temp = ref MemoryMarshal.GetReference(AcquireSpan(1));
      _position++;
      return ref temp;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Write(scoped in T value)
   {
      AcquireSpan(1)[0] = value;
      _position++;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Write(scoped ReadOnlySpan<T> span)
   {
      if (span.IsEmpty) return;

      span.CopyTo(
         FreeCapacity >= span.Length 
            ? _owner.Span[_position..] 
            : AcquireSpan(span.Length));

      _position += span.Length;
   }
   
   private Span<T> AcquireSpan(int requestedLength)
   {
      if (FreeCapacity >= requestedLength)
      {
         return _owner.Span[_position..];
      }

      int newLength;
      if (!_isGrown && _initialMinGrowCapacity >= requestedLength)
      {
         newLength = _initialMinGrowCapacity;
      }
      else
      {
         var growBy = _owner.Length > 0 ? Math.Max(requestedLength, _owner.Length) : 256;
         newLength = _owner.Length + growBy;
      }

      var lastOwner = _owner;
      _owner = BufferAllocator<T>.CreatePooled(newLength, true);
      
      lastOwner.Span.CopyTo(_owner.Span);
      lastOwner.Dispose();
      
      _isGrown = true;
      return _owner.Span[_position..];
   }

   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }
      
      _disposed = true;
      _owner.Dispose();
   }
}