using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.CodeGen;

namespace CodeGen.Common.Buffers;

[StructLayout(LayoutKind.Sequential)]
public ref struct CodeTextWriter : IDisposable
{
   public ReadOnlySpan<char> WrittenSpan
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _buffer.WrittenSpan;
   }

   public char IndentCharacter
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _indentCharacter;
   }
   
   public char NewLineCharacter
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _newLineCharacter;
   }
   
   public int IndentCount
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _indentCount;
   }
   
   public int CurrentIndentLevel
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _currentLevel;
   }
   
   private const char DefaultIndent = '\t';
   private const char DefaultNewLine = '\n';

   private readonly char _indentCharacter;
   private readonly char _newLineCharacter;
   
   private BufferWriter<char> _indentCache;
   private int _currentLevel;
   private readonly int _indentCount;
   private ReadOnlySpan<char> _currentLevelBuffer;

   private BufferWriter<char> _buffer;
   
   public CodeTextWriter(
      Span<char> buffer,
      Span<char> indentBuffer,
      int indentCount = 1,
      char indentCharacter = DefaultIndent,
      char newLineCharacter = DefaultNewLine,
      int initialMinGrowCapacity = 1024)
   {
      _indentCharacter = indentCharacter;
      _indentCount = indentCount;
      
      _newLineCharacter = newLineCharacter;
      _buffer = new BufferWriter<char>(buffer, initialMinGrowCapacity);
      
      _currentLevel = 0;
      
      _indentCache = new BufferWriter<char>(indentBuffer);
      _indentCache.Fill(_indentCharacter);
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
   public void WriteLine()
   {
      _buffer.Add(_newLineCharacter);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition)
   {
      if (condition)
      {
         WriteLine();
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLineIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         WriteLine(content, multiLine);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped Span<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(string text)
   {
      WriteText(text.AsSpan());
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(scoped ReadOnlySpan<char> text)
   {
      AddIndentOnDemand();
      _buffer.Write(text);
   }

   public void Write(scoped ReadOnlySpan<char> text, bool multiLine = false)
   {
      if (!multiLine)
      {
         WriteText(text);
      }
      else
      {
         while (text.Length > 0)
         {
            var newLinePos = text.IndexOf(_newLineCharacter);

            if (newLinePos >= 0)
            {
               var line = text[..newLinePos];
               
               WriteIf(!line.IsEmpty, line);
               WriteLine();

               text = text[(newLinePos + 1)..];
            }
            else
            {
               WriteText(text);
               break;
            }
         }
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         Write(content, multiLine);
      }  
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void UpIndent()
   {
      _currentLevel++;
      _currentLevelBuffer = GetCurrentIndentBuffer();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void DownIndent()
   {
      _currentLevel--;
      ArgumentOutOfRangeException.ThrowIfLessThan(_currentLevel, 0, nameof(_currentLevel));
      
      _currentLevelBuffer = GetCurrentIndentBuffer();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpan(int length)
   {
      return _buffer.AcquireSpan(length, true);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<char> AcquireSpanIndented(int length)
   {
      AddIndentOnDemand();
      return _buffer.AcquireSpan(length, true);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<char> GetCurrentIndentBuffer()
   {
      if (_currentLevel == 0)
      {
         return [];
      }

      var levelCount = _indentCount * _currentLevel;
      
      while (_indentCache.Position < levelCount)
      {
         _indentCache.Add(_indentCharacter);
      }

      return _indentCache.WrittenSpan[..levelCount];
   }

   private void AddIndentOnDemand()
   {
      if (_currentLevelBuffer.IsEmpty)
      {
         return;
      }
      
      if (_buffer.Position == 0 || _buffer.WrittenSpan[^1] == _newLineCharacter)
      {
         _buffer.Write(_currentLevelBuffer);
      }
   }

   public override string ToString()
   {
      return _buffer.WrittenSpan.Trim().ToString();
   }

   public void Dispose()
   {
      _buffer.Dispose();
      _indentCache.Dispose();
   }
}