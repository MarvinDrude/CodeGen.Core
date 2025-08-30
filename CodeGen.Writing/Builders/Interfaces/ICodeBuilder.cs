using System.Runtime.CompilerServices;

namespace CodeGen.Writing.Builders.Interfaces;

public interface ICodeBuilder
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal ref CodeBuilder GetBuilder();
}

public static class CodeBuilderExtensions
{
   extension<T>(ref T builder)
      where T : struct, ICodeBuilder, allows ref struct
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLine(string line)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLine(line);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLine()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLine();
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLineIf(bool condition)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLineIf(condition);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLineIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLineIf(condition, content, multiLine);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLine(scoped Span<char> content, bool multiLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLine(content, multiLine);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteLine(scoped ReadOnlySpan<char> content, bool multiLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteLine(content, multiLine);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteText(string text)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteText(text);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T Write(scoped ReadOnlySpan<char> text, bool multiLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.Write(text, multiLine);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteIf(bool condition, scoped ReadOnlySpan<char> content, bool multiLine = false)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.WriteIf(condition, content, multiLine);
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T UpIndent()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.UpIndent();
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T DownIndent()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.DownIndent();
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T OpenBody()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.OpenBody();
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T CloseBody()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.CloseBody();
         
         return ref builder;
      }
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T CloseBodySemicolon()
      {
         ref var writer = ref builder.GetBuilder().Writer;
         writer.CloseBodySemicolon();
         
         return ref builder;
      }
   }
}