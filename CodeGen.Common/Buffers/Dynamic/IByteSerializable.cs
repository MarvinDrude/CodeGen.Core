namespace CodeGen.Common.Buffers.Dynamic;

public interface IByteSerializable<T>
   where T : allows ref struct
{
   static abstract void Write(scoped Span<byte> buffer, scoped ref readonly T instance);
   static abstract void Read(scoped ReadOnlySpan<byte> buffer, out T instance);

   static abstract int CalculateByteLength(scoped ref readonly T instance);
}