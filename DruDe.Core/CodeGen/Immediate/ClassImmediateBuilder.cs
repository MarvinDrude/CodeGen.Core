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
   public static ref ClassImmediateBuilder OpenHeader(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> declaration)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(declaration);
      
      return ref self;
   }
   
   public static ref ClassImmediateBuilder CloseHeader(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine("{");
      writer.UpIndent();
      
      return ref self;
   }
   
   public static ref ClassImmediateBuilder FirstBaseDeclaration(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> name, bool close = false)
   {
      ref var writer = ref self.Builder.Writer;
      ReadOnlySpan<char> start = ": ";
      
      writer.UpIndent();
      writer.Write(start);
      writer.Write(name);
      
      if (close)
      {
         return ref self.CloseBaseDeclaration();
      }
      return ref self;
   }
   
   public static ref ClassImmediateBuilder NextBaseDeclaration(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> name, bool close = false)
   {
      ref var writer = ref self.Builder.Writer;
      ReadOnlySpan<char> space = "  ";
      
      writer.WriteLine(",");
      writer.Write(space);
      writer.Write(name);
      
      if (close)
      {
         return ref self.CloseBaseDeclaration();
      }
      return ref self;
   }
   
   public static ref ClassImmediateBuilder CloseBaseDeclaration(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine();
      writer.DownIndent();
      
      return ref self;
   }
   
   public static ref ClassImmediateBuilder FirstGenericConstraint(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> constraint)
   {
      ref var writer = ref self.Builder.Writer;

      writer.UpIndent();
      writer.WriteLine(constraint);
      
      return ref self;
   }
   
   public static ref ClassImmediateBuilder NextGenericConstraint(
      this ref ClassImmediateBuilder self, scoped ReadOnlySpan<char> constraint)
   {
      ref var writer = ref self.Builder.Writer;

      writer.WriteLine(constraint);
      
      return ref self;
   }
   
   public static ref ClassImmediateBuilder CloseGenericConstraint(
      this ref ClassImmediateBuilder self)
   {
      ref var writer = ref self.Builder.Writer;
      writer.DownIndent();
      
      return ref self;
   }
}