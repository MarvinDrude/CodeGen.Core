using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Contracts.Constants;
using Me.Memory.Buffers;

namespace CodeGen.Writing;

[StructLayout(LayoutKind.Auto)]
public ref partial struct CodeTextWriter : IDisposable
{
   public ReadOnlySpan<char> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _writer.WrittenSpan;
   }
   
   private TextWriterIndentSlim _writer;

   public CodeTextWriter(
      Span<char> buffer,
      Span<char> indentBuffer,
      int indentSize = DefaultIndentSize,
      char indentChar = DefaultIndent,
      char newLineChar = DefaultNewLine,
      int initialMinGrowCapacity = 1024)
   {
      _writer = new TextWriterIndentSlim(
         buffer,
         indentBuffer,
         indentSize,
         indentChar,
         newLineChar,
         initialMinGrowCapacity);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void OpenBody()
   {
      WriteLine(StringConstants.OpenCurlyBracket);
      UpIndent();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CloseBody()
   {
      DownIndent();
      WriteLine(StringConstants.CloseCurlyBracket);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CloseBodySemicolon()
   {
      DownIndent();
      WriteLine(StringConstants.CloseCurlyBracketSemicolon);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine() => _writer.WriteLine();
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition) => _writer.WriteLineIf(condition);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
      => _writer.WriteLineIf(condition, content, multiLine);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped Span<char> content, bool multiLine = false)
      => _writer.WriteLine(content, multiLine);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped ReadOnlySpan<char> content, bool multiLine = false)
      => _writer.WriteLine(content, multiLine);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(string text)
      => _writer.WriteText(text);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(scoped ReadOnlySpan<char> text)
      => _writer.WriteText(text);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Write(scoped ReadOnlySpan<char> text, bool multiLine = false)
      => _writer.Write(text, multiLine);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
      => _writer.WriteIf(condition, content, multiLine);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void UpIndent()
      => _writer.UpIndent();
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void DownIndent()
      => _writer.DownIndent();
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpan(int length)
      => _writer.AcquireSpan(length);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpanIndented(int length)
      => _writer.AcquireSpanIndented(length);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<char> GetCurrentIndentBuffer()
      => _writer.GetCurrentIndentBuffer();

   public override string ToString()
   {
      return _writer.WrittenSpan.ToString();
   }
   
   public void Dispose()
   {
      _writer.Dispose();
   }
}