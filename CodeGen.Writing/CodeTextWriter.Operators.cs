using System.Runtime.CompilerServices;

namespace CodeGen.Writing;

public ref partial struct CodeTextWriter
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void operator += (char element) => _writer += element;
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void operator += (ReadOnlySpan<char> span) => _writer += span;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void operator <<= (char element) => _writer <<= element;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void operator <<= (ReadOnlySpan<char> span) => _writer <<= span;
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void operator |= (ReadOnlySpan<char> span) => _writer |= span;
}