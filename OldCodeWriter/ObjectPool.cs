namespace OldCodeWriter;

internal sealed class ObjectPool<T>
   where T : class
{
   private readonly Element[] _items;
   private readonly Func<T> _factory;
   private T? _firstItem;
   
   public ObjectPool(Func<T> factory, int capacity)
   {
      _factory = factory;
      _items = new Element[capacity];
   }

   public void Return(T ob)
   {
      if(_firstItem is null)
      {
         _firstItem = ob;
      }
      else
      {
         ReturnLoop(ob);
      }
   }

   public T Get()
   {
      var item = _firstItem;

      if(item is null || item != Interlocked.CompareExchange(ref _firstItem, null, item))
      {
         item = GetLoop();
      }

      return item;
   }

   private void ReturnLoop(T ob)
   {
      foreach(ref var element in _items.AsSpan())
      {
         if(element.Value is not null)
         {
            continue;
         }

         element.Value = ob;
         break;
      }

   }

   private T GetLoop()
   {
      foreach(ref var element in _items.AsSpan())
      {
         if(element.Value is not { } current)
         {
            continue;
         }

         if(current == Interlocked.CompareExchange(ref element.Value, null, current))
         {
            return current;
         }
      }

      return _factory();
   }

   private struct Element
   {
      internal T? Value;
   }
}