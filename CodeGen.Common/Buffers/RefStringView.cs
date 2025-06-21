using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Common.Buffers;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct RefStringView : IByteSerializable<RefStringView>
{
   public ReadOnlySpan<char> Span
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _span;
   }

   public int Length
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _span.Length;
   }

   private readonly ReadOnlySpan<char> _span;
   
   public RefStringView(ReadOnlySpan<char> span)
   {
      _span = span;
   }

   public override string ToString()
   {
      return _span.ToString();
   }
   
   public static implicit operator RefStringView(string str) => new (str);
   public static implicit operator RefStringView(ReadOnlySpan<char> span) => new (span);
   
   public static void Write(scoped Span<byte> buffer, scoped ref readonly RefStringView instance)
   {
      var length = instance.Length * sizeof(char);
      using var writer = new ByteWriter(buffer);

      writer.WriteLittleEndian(length);
      writer.WriteStringRaw(instance.Span);
   }

   public static void Read(ReadOnlySpan<byte> buffer, out RefStringView instance)
   {
      var reader = new ByteReader(buffer);
      var length = reader.ReadLittleEndian<int>();
      var chars = reader.ReadStringRaw(length);
      
      instance = new RefStringView(chars);
   }

   public static int CalculateByteLength(scoped ref readonly RefStringView instance)
   {
      return sizeof(int) + instance.Length * sizeof(char);
   }
}