namespace CodeGen.Common.Buffers.Dynamic;

public interface IByteSerializable<T>
   where T : allows ref struct
{
   static abstract void Write(scoped Span<byte> buffer, scoped in T instance);
   static abstract int Read(ReadOnlySpan<byte> buffer, out T instance);

   static abstract int CalculateByteLength(scoped in T instance);
}