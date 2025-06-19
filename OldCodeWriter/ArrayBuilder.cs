using System.Buffers;

namespace OldCodeWriter;

internal struct ArrayBuilder<T> : IDisposable
{
   private static readonly ObjectPool<ArrayWriter> WriterPool = new(static () => new ArrayWriter(), 64);

   public readonly ReadOnlySpan<T> Span => Writer.Span;
   public readonly int Count => Writer.Count;

   private bool _disposed = false;

   private ArrayWriter Writer { get; set; }

   public ArrayBuilder()
   {
      Writer = WriterPool.Get();
   }

   public readonly Span<T> Advance(int size)
   {
      return Writer.Advance(size);
   }

   public readonly void Add(T item)
   {
      Writer.Add(item);
   }

   public readonly void AddRange(ReadOnlySpan<T> items)
   {
      Writer.AddRange(items);
   }

   public readonly void Insert(int index, T item)
   {
      Writer.Insert(index, item);
   }

   public readonly T[] ToArray()
   {
      return Writer.Span.ToArray();
   }

   public readonly override string ToString()
   {
      return Writer.Span.ToString();
   }

   public readonly void Clear()
   {
      Writer.Clear();
   }

   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }

      _disposed = true;

      Writer.Clear();
      WriterPool.Return(Writer);

      Writer = null!;
   }

   private sealed class ArrayWriter
   {
      public int Count => _index;

      public T this[int index] => Span[_index];

      public ReadOnlySpan<T> Span => new(_contents, 0, _index);

      private T[] _contents;
      private int _index;

      public ArrayWriter()
      {
         var size = (uint)(typeof(T) == typeof(char) ? 1024 : 16);
         _contents = new T[size];
         _index = 0;
      }

      public void Add(T value)
      {
         EnsureCapacity(1);
         _contents[_index++] = value;
      }

      public Span<T> Advance(int size)
      {
         EnsureCapacity(size);
         var span = _contents.AsSpan(_index, size);

         _index += size;

         return span;
      }

      public void Insert(int index, T item)
      {
         if (index < 0 || index > _index)
         {
            throw new ArgumentOutOfRangeException(nameof(index));
         }

         EnsureCapacity(1);

         if (_index < index)
         {
            Array.Copy(_contents, index, _contents, index + 1, _index - index);
         }

         _contents[index] = item;
         _index++;
      }

      public void AddRange(ReadOnlySpan<T> items)
      {
         EnsureCapacity(items.Length);
         items.CopyTo(_contents.AsSpan(_index));

         _index += items.Length;
      }

      public void Clear()
      {
         _contents.AsSpan(0, _index).Clear();
         _index = 0;
      }

      private void EnsureCapacity(int capacity)
      {
         if (capacity > _contents.Length - _index)
         {
            ResizeContents(capacity);
         }
      }

      private void ResizeContents(int capacity)
      {
         var minCapacity = _index + capacity;
         var newCapacity = Math.Max(minCapacity, _contents.Length * 2);

         var newContents = new T[newCapacity];
         Array.Copy(_contents, newContents, _index);

         _contents = newContents;
      }
   }
}