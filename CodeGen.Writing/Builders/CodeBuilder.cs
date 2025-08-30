using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Writing.Builders.Common;
using CodeGen.Writing.Builders.Interfaces;

namespace CodeGen.Writing.Builders;

[StructLayout(LayoutKind.Sequential)]
public ref struct CodeBuilder : ICodeBuilder
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

      ref var self = ref Unsafe.AsRef(ref this);
      File = new FileBuilder(ref self);
   }

   ref CodeBuilder ICodeBuilder.GetBuilder()
   {
      return ref Unsafe.AsRef(ref this);
   }
}