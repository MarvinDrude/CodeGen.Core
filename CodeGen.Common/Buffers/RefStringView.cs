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
      
   }

   public static void Read(scoped ReadOnlySpan<byte> buffer, out RefStringView instance)
   {
      throw new NotImplementedException();
   }

   public static int CalculateByteLength(scoped ref readonly RefStringView instance)
   {
      throw new NotImplementedException();
   }
}