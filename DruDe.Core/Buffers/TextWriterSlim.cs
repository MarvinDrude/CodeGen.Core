using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.Buffers;

[StructLayout(LayoutKind.Auto)]
public ref struct TextWriterSlim : IDisposable
{
   private BufferWriter<char> _buffer;

   public TextWriterSlim(Span<char> buffer)
   {
      _buffer = new BufferWriter<char>(buffer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void WriteText(scoped in ReadOnlySpan<char> text)
   {
      _buffer.Write(text);
   }

   public void Dispose()
   {
      _buffer.Dispose();
   }
}