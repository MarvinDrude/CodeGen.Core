using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGen.Writing.Builders.Interfaces;

public static class CodeBuilderInterpolateExtensions
{
   public static ref T WriteInterpolated<T>(this ref T builder, IFormatProvider? provider,
      [InterpolatedStringHandlerArgument(nameof(builder), nameof(provider))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, ICodeBuilder, allows ref struct
   {
      return ref builder;
   }
   
   public static ref T WriteLineInterpolated<T>(this ref T builder, IFormatProvider? provider,
      [InterpolatedStringHandlerArgument(nameof(builder), nameof(provider))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, ICodeBuilder, allows ref struct
   {
      builder.WriteLine();
      return ref builder;
   }

   public static ref T WriteInterpolated<T>(this ref T builder,
      [InterpolatedStringHandlerArgument(nameof(builder))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, ICodeBuilder, allows ref struct
   {
      return ref builder;
   }
   
   public static ref T WriteLineInterpolated<T>(this ref T builder,
      [InterpolatedStringHandlerArgument(nameof(builder))]
      scoped ref CodeBuilderInterpolatedStringHandler<T> handler)
      where T : struct, ICodeBuilder, allows ref struct
   {
      builder.WriteLine();
      return ref builder;
   }
}

[InterpolatedStringHandler]
[EditorBrowsable(EditorBrowsableState.Never)]
[StructLayout(LayoutKind.Auto)]
public unsafe ref struct CodeBuilderInterpolatedStringHandler<T>
   where T : struct, ICodeBuilder, allows ref struct
{
   private readonly nint _writerPointer;
   private ref CodeTextWriter Writer
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.AsRef<CodeTextWriter>((void*)_writerPointer);
   }

   public int Count { get; private set; }

   private readonly IFormatProvider? _provider;

   public CodeBuilderInterpolatedStringHandler(
      int literalLength,
      int formattedCount,
      scoped ref T builder,
      IFormatProvider? provider = null)
   {
      _writerPointer = (nint)Unsafe.AsPointer(ref builder.GetBuilder().Writer);
      
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

   public void AppendFormatted<TType>(TType value, string? format = null)
   {
      Count += AppendFormattedInternal(value, format);
   }
   
   private int AppendFormattedInternal<TType>(TType value, string? format)
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