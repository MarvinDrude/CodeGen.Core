using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DruDe.Core.CodeGen.Immediate;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct MethodImmediateBuilder : IImmediateBuilder
{
   private readonly ref byte _builderReference;
   public ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }

   public MethodImmediateBuilder(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
   }
}

public static class MethodImmediateBuilderExtensions
{
   public static ref MethodImmediateBuilder OpenHeader(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> modifiers,
      scoped ReadOnlySpan<char> returnType,
      scoped ReadOnlySpan<char> name,
      bool hasParameters = true)
   {
      ref var writer = ref self.Builder.Writer;
      var totalSize = modifiers.Length + name.Length + returnType.Length + 2;

      if (totalSize <= 1024)
      {
         Span<char> buffer = stackalloc char[totalSize];
         var completeBuffer = buffer;
         
         modifiers.CopyTo(buffer);
         buffer = buffer[modifiers.Length..];

         buffer[0] = ' ';
         buffer = buffer[1..];
         
         returnType.CopyTo(buffer);
         buffer = buffer[returnType.Length..];
         
         buffer[0] = ' ';
         buffer = buffer[1..];
         
         name.CopyTo(buffer);
         writer.Write(completeBuffer);
      }
      else
      {
         writer.Write(modifiers);
         writer.Write(" ");
         writer.Write(returnType);
         writer.Write(" ");
         writer.Write(name);
      }
      
      if (hasParameters)
      {
         writer.WriteLine("(");
      }
      else
      {
         writer.Write("(");
      }
      
      return ref self;
   }
   
   public static ref MethodImmediateBuilder CloseHeader(
      this ref MethodImmediateBuilder self, bool semicolon = false)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(!semicolon ? ")" : ");");

      if (!semicolon)
      {
         writer.DownIndent();
      }

      return ref self;
   }
   
   public static ref MethodImmediateBuilder CloseHeaderNoParameters(
      this ref MethodImmediateBuilder self, bool semicolon = false)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(!semicolon ? ")" : ");");

      return ref self;
   }

   public static ref MethodImmediateBuilder FirstParameter(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> type,
      scoped ReadOnlySpan<char> name)
   {
      ref var writer = ref self.Builder.Writer;
      
      writer.UpIndent();
      writer.Write(type);
      writer.Write(" ");
      writer.Write(name);
      
      return ref self;
   }
   
   public static ref MethodImmediateBuilder NextParameter(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> type,
      scoped ReadOnlySpan<char> name)
   {
      ref var writer = ref self.Builder.Writer;
      
      writer.WriteLine(",");
      writer.Write(type);
      writer.Write(" ");
      writer.Write(name);
      
      return ref self;
   }
}
