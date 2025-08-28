using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Me.Memory.Buffers;

namespace CodeGen.Writing;

public static class TextWriterIndentSlimInterpolatedStringHandlerExtensions
{
   public static int WriteInterpolated(this ref CodeTextWriter writer, IFormatProvider? provider,
      [InterpolatedStringHandlerArgument(nameof(writer), nameof(provider))]
      scoped in CodeTextWriterInterpolatedStringHandler handler)
   {
      return handler.Count;
   }
   
   public static int WriteInterpolated(this ref CodeTextWriter writer,
      [InterpolatedStringHandlerArgument(nameof(writer))]
      scoped in CodeTextWriterInterpolatedStringHandler handler)
   {
      return handler.Count;
   }
}

[InterpolatedStringHandler]
[EditorBrowsable(EditorBrowsableState.Never)]
[StructLayout(LayoutKind.Auto)]
public ref struct CodeTextWriterInterpolatedStringHandler
{
   private readonly ref byte _writerReference;
   public ref CodeTextWriter Writer
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeTextWriter>(ref _writerReference);
   }

   public int Count { get; private set; }

   private readonly IFormatProvider? _provider;

   public CodeTextWriterInterpolatedStringHandler(
      int literalLength,
      int formattedCount,
      ref CodeTextWriter writer,
      IFormatProvider? provider = null)
   {
      _writerReference = ref Unsafe.As<CodeTextWriter, byte>(ref writer);

      _provider = provider;
      Count = 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AppendLiteral(string? value)
   {
      if (value is null) return;
      AppendFormatted(value.AsSpan());
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AppendFormatted(string value)
   {
      AppendFormatted(value.AsSpan());
   }
   
   public void AppendFormatted(scoped ReadOnlySpan<char> value)
   {
      Writer.Write(value);
      Count += 0;
   }

   public void AppendFormatted<T>(T value, string? format = null)
   {
      Count += AppendFormattedInternal(value, format);
   }
   
   private int AppendFormattedInternal<T>(T value, string? format)
   {
      var charsWritten = value switch
      {
         IFormattable formattable => Write(ref Writer, formattable.ToString(format, _provider)),
         not null => Write(ref Writer, value.ToString()),
         _ => 0
      };

      return charsWritten;

      static int Write(scoped ref CodeTextWriter writer, scoped ReadOnlySpan<char> chars)
      {
         writer.Write(chars);
         return chars.Length;
      }
   }

   public override string ToString() => Writer.WrittenSpan.ToString();
}