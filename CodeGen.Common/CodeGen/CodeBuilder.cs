using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen;

[StructLayout(LayoutKind.Sequential)]
public ref struct CodeBuilder : IDisposable
{
   public CodeTextWriter Writer;
   
   public NameSpaceImmediateBuilder NameSpaceIm;
   public ClassImmediateBuilder ClassIm;
   public MethodImmediateBuilder MethodIm;

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
      ClassIm = new ClassImmediateBuilder(ref Unsafe.AsRef(ref this));
      MethodIm = new MethodImmediateBuilder(ref Unsafe.AsRef(ref this));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public override string ToString()
   {
      return Writer.ToString();
   }
   
   public void Dispose()
   {
      Writer.Dispose();
   }
}