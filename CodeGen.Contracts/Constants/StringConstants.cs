using System.Runtime.CompilerServices;

namespace CodeGen.Contracts.Constants;

public static class StringConstants
{
   public static ReadOnlySpan<char> Space
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => " ".AsSpan();
   }

   public static ReadOnlySpan<char> TwoSpace
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => "  ".AsSpan();
   }

   public static ReadOnlySpan<char> Comma
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ",".AsSpan();
   }

   public static ReadOnlySpan<char> OpenCurlyBracket
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => "{".AsSpan();
   }

   public static ReadOnlySpan<char> CloseCurlyBracket
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => "}".AsSpan();
   }

   public static ReadOnlySpan<char> CloseCurlyBracketSemicolon
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => "};".AsSpan();
   }

   public static ReadOnlySpan<char> OpenParenthese
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => "(".AsSpan();
   }

   public static ReadOnlySpan<char> CloseParenthese
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ")".AsSpan();
   }

   public static ReadOnlySpan<char> CloseParentheseSemicolon
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ");".AsSpan();
   }

   public static ReadOnlySpan<char> Colon
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ":".AsSpan();
   }

   public static ReadOnlySpan<char> ColonSpace
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ": ".AsSpan();
   }

   public static ReadOnlySpan<char> Semicolon
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ";".AsSpan();
   }
}