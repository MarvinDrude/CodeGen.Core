using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Common.Buffers;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct RefStringView
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
}