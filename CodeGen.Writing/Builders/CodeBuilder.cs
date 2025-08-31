using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Writing.Builders.Common;
using CodeGen.Writing.Builders.Interfaces;
using CodeGen.Writing.Builders.Types;

namespace CodeGen.Writing.Builders;

[StructLayout(LayoutKind.Sequential)]
public ref struct CodeBuilder : ICodeBuilder, IDisposable
{
   public CodeTextWriter Writer;

   public FileBuilder File;
   public TypeHeaderBuilder TypeHeader;
   
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
      TypeHeader = new TypeHeaderBuilder(ref self);
   }

   ref CodeBuilder ICodeBuilder.GetBuilder()
   {
      return ref Unsafe.AsRef(ref this);
   }

   public void Dispose()
   {
      Writer.Dispose();
   }
}