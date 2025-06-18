using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DruDe.Core.Buffers;
using DruDe.Core.CodeGen.Immediate;

namespace DruDe.Core.CodeGen;

[StructLayout(LayoutKind.Auto)]
public ref struct CodeBuilder : IDisposable
{
   public CodeTextWriter Writer;

   public NameSpaceImmediateBuilder NameSpaceIm;

   public CodeBuilder(
      Span<char> buffer,
      Span<char> indentBuffer,
      int indentCount = 3,
      char indentCharacter = ' ',
      char newLineCharacter = '\n')
   {
      Writer = new CodeTextWriter(
         buffer, indentBuffer, 
         indentCount, indentCharacter,
         newLineCharacter);
      
      NameSpaceIm = new NameSpaceImmediateBuilder(ref Unsafe.AsRef(ref this));
   }
   
   public void Dispose()
   {
      Writer.Dispose();
   }
}