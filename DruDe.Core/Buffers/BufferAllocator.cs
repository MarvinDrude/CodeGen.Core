﻿using System.Buffers;

namespace DruDe.Core.Buffers;

public static class BufferAllocator<T>
{
   private static readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;

   public static BufferOwner<T> CreatePooled(int minBufferLength, bool exactLength)
   {
      var owner = new BufferOwner<T>(_pool, minBufferLength, exactLength);
      return owner;
   }

   public static BufferOwner<T> Create(Span<T> buffer)
   {
      var owner = new BufferOwner<T>(buffer);
      return owner;
   }
}