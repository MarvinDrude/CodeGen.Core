using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct FluentCodeBuilder : IDisposable
{
   internal CodeBuilder Builder;
   internal BufferOwner<byte> TempOwner;

   public FluentCodeBuilder(
      Span<char> textBuffer,
      Span<char> indentBuffer,
      Span<byte> temporaryData,
      int indentCount = 3,
      char indentChar = ' ',
      char newLineChar = '\n',
      int initialMinGrowCapacity = 1024)
   {
      TempOwner = BufferAllocator<byte>.Create(temporaryData);
      Builder = new CodeBuilder(
         textBuffer,
         indentBuffer,
         indentCount,
         indentChar,
         newLineChar,
         enableStateBuilders: true,
         initialMinGrowCapacity);
   }

   public ClassBuilderInfo CreateClass()
   {
      return new ClassBuilderInfo(ref Unsafe.AsRef(ref this));
   }

   public void Dispose()
   {
      Builder.Dispose();
   }
}