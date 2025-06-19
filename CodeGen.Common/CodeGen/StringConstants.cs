namespace CodeGen.Common.CodeGen;

public static class StringConstants
{
   public static ReadOnlySpan<char> Space => " ".AsSpan();
   public static ReadOnlySpan<char> TwoSpace => "  ".AsSpan();
   
   public static ReadOnlySpan<char> Comma => ",".AsSpan();
   
   public static ReadOnlySpan<char> OpenCurlyBracket => "{".AsSpan();
   public static ReadOnlySpan<char> CloseCurlyBracket => "}".AsSpan();
   public static ReadOnlySpan<char> CloseCurlyBracketSemicolon => "};".AsSpan();
   
   public static ReadOnlySpan<char> OpenParenthese => "(".AsSpan();
   public static ReadOnlySpan<char> CloseParenthese => ")".AsSpan();
   public static ReadOnlySpan<char> CloseParentheseSemicolon => ");".AsSpan();
   
   
   public static ReadOnlySpan<char> Colon => ":".AsSpan();
   public static ReadOnlySpan<char> ColonSpace => ": ".AsSpan();
   
   public static ReadOnlySpan<char> Semicolon => ";".AsSpan();
}