using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.CodeGen.Immediate;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ClassImmediateBuilder : IImmediateBuilder
{
   private readonly ref byte _builderReference;
   public ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }

   public ClassImmediateBuilder(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
   }
}

public static class ClassImmediateBuilderExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder OpenHeader(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> declaration)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(declaration);
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder CloseHeader(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine(StringConstants.OpenCurlyBracket);
      writer.UpIndent();
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder FirstBaseDeclaration(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> name, bool close = false)
   {
      ref var writer = ref self.Builder.Writer;
      var start = StringConstants.ColonSpace;
      
      writer.UpIndent();

      var totalSize = start.Length + name.Length;

      if (totalSize <= 1024)
      {
         Span<char> buffer = stackalloc char[totalSize];
         var completeBuffer = buffer;

         start.CopyTo(buffer);
         buffer = buffer[start.Length..];

         name.CopyTo(buffer);
         writer.Write(completeBuffer);
      }
      else
      {
         writer.Write(start);
         writer.Write(name);
      }
      
      if (close)
      {
         return ref self.CloseBaseDeclaration();
      }
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder NextBaseDeclaration(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> name, bool close = false)
   {
      ref var writer = ref self.Builder.Writer;
      var totalSize = name.Length + 2 + 2 + writer.IndentCount * writer.CurrentIndentLevel;

      if (totalSize <= 1024)
      {
         Span<char> buffer = stackalloc char[totalSize];
         var completeBuffer = buffer;

         buffer[0] = ',';
         buffer[1] = writer.NewLineCharacter;
         buffer = buffer[2..];

         var indent = writer.GetCurrentIndentBuffer();
         indent.CopyTo(buffer);
         buffer = buffer[indent.Length..];
         
         buffer[0] = ' ';
         buffer[1] = ' ';
         buffer = buffer[2..];

         name.CopyTo(buffer);
         
         writer.Write(completeBuffer);
      }
      else
      {
         writer.WriteLine(StringConstants.Comma);
         writer.Write(StringConstants.TwoSpace);
         writer.Write(name);
      }
      
      if (close)
      {
         return ref self.CloseBaseDeclaration();
      }
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder CloseBaseDeclaration(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine();
      writer.DownIndent();
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder FirstGenericConstraint(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> constraint)
   {
      ref var writer = ref self.Builder.Writer;

      writer.UpIndent();
      writer.WriteLine(constraint);
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder NextGenericConstraint(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> constraint)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine(constraint);
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassImmediateBuilder CloseGenericConstraint(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;
      writer.DownIndent();
      
      return ref self;
   }
}