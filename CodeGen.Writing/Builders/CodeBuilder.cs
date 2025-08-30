using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Writing.Builders.Common;

namespace CodeGen.Writing.Builders;

[StructLayout(LayoutKind.Sequential)]
public ref struct CodeBuilder
{
   public CodeTextWriter Writer;

   public FileBuilder File;

   public CodeBuilder(
      Span<char> buffer,
      Span<char> indentBuffer,
      int indentSize = CodeTextWriter.DefaultIndentSize,
      char indentChar = CodeTextWriter.DefaultIndent,
      char newLineChar = CodeTextWriter.DefaultNewLine,
      int initialMinGrowCapacity = 1024)
   {
      Writer = new CodeTextWriter(
         buffer,
         indentBuffer,
         indentSize,
         indentChar,
         newLineChar,
         initialMinGrowCapacity);

      File = new FileBuilder(ref Unsafe.AsRef(ref this));
   }
}