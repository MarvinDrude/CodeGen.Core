using System.Runtime.CompilerServices;

namespace DruDe.Core.CodeGen.Immediate;

public interface IImmediateBuilder
{
   public ref CodeBuilder Builder { get; }
}

public static class ImmediateBuilderExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T OpenBody<T>(this ref T self) 
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.OpenBody();
      return ref self;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T CloseBody<T>(this ref T self)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.CloseBody();
      return ref self;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T WriteLine<T>(this ref T self, scoped ReadOnlySpan<char> line)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.WriteLine(line);
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T WriteLine<T>(this ref T self)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.WriteLine();
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T Write<T>(this ref T self, scoped ReadOnlySpan<char> text, bool multiLine = false)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.Write(text, multiLine);
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T WriteLineIf<T>(this ref T self, bool condition, scoped ReadOnlySpan<char> line)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.WriteLineIf(condition, line);
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T WriteLineIf<T>(this ref T self, bool condition)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      self.Builder.Writer.WriteLineIf(condition);
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref CodeBuilder Done<T>(this ref T self)
      where T : struct, IImmediateBuilder, allows ref struct
   {
      return ref self.Builder;
   }
}