using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.Buffers;

[StructLayout(LayoutKind.Auto)]
public ref struct CodeTextWriter : IDisposable
{
   private const char DefaultIndent = '\t';
   private const char DefaultNewLine = '\n';

   private readonly char _indentCharacter;
   private readonly char _newLineCharacter;
   
   private BufferWriter<char> _indentCache;
   private int _currentLevel;
   private ReadOnlySpan<char> _currentLevelBuffer;

   private BufferWriter<char> _buffer;
   
   public CodeTextWriter(
      Span<char> buffer,
      Span<char> indentBuffer,
      char indentCharacter = DefaultIndent,
      char newLineCharacter = DefaultNewLine)
   {
      _indentCharacter = indentCharacter;
      _newLineCharacter = newLineCharacter;

      _buffer = new BufferWriter<char>(buffer);
      
      _currentLevel = 0;
      _indentCache = new BufferWriter<char>(indentBuffer);
      for (var e = 0; e < indentBuffer.Length; e++)
      {
         _indentCache.Add(_indentCharacter);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void OpenBody()
   {
      WriteLine("{");
      UpIndent();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CloseBody()
   {
      WriteLine("}");
      DownIndent();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CloseBodySemicolon()
   {
      WriteLine("};");
      DownIndent();
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

   public void WriteLineIf(bool condition, scoped in ReadOnlySpan<char> content, bool multiLine = false)
   {
      if (condition)
      {
         WriteLine(content, multiLine);
      }
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteLine(scoped in ReadOnlySpan<char> content, bool multiLine = false)
   {
      Write(content, multiLine);
      WriteLine();
   }
   
   public void WriteText(string text)
   {
      WriteText(text.AsSpan());
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(scoped in ReadOnlySpan<char> text)
   {
      AddIndentOnDemand();
      _buffer.Write(text);
   }

   public void Write(scoped in ReadOnlySpan<char> text, bool multiLine = false)
   {
      if (!multiLine)
      {
         WriteText(text);
      }
      else
      {
         var copyText = text;
         
         while (copyText.Length > 0)
         {
            var newLinePos = copyText.IndexOf(_newLineCharacter);

            if (newLinePos >= 0)
            {
               var line = copyText[..newLinePos];
               
               WriteIf(!line.IsEmpty, line);
               WriteLine();

               copyText = copyText[(newLinePos + 1)..];
            }
            else
            {
               WriteText(copyText);
               break;
            }
         }
      }
   }

   public void WriteIf(bool condition, scoped in ReadOnlySpan<char> content, bool multiLine = false)
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
   
   private ReadOnlySpan<char> GetCurrentIndentBuffer()
   {
      if (_currentLevel == 0)
      {
         return [];
      }

      while (_indentCache.Position < _currentLevel)
      {
         _indentCache.Add(_indentCharacter);
      }

      return _indentCache.WrittenSpan[.._currentLevel];
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