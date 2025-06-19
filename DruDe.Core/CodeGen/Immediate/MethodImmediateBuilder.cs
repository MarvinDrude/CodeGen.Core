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
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodImmediateBuilder OpenHeader(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> modifiers,
      scoped ReadOnlySpan<char> returnType,
      scoped ReadOnlySpan<char> name,
      bool hasParameters = true)
   {
      ref var writer = ref self.Builder.Writer;
      var totalSize = modifiers.Length + name.Length + returnType.Length + 2 + (hasParameters ? 2 : 1);

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
         buffer = buffer[name.Length..];
         buffer[0] = '(';
         
         if (hasParameters)
         {
            buffer[1] = writer.NewLineCharacter;
         }
         
         writer.Write(completeBuffer);
      }
      else
      {
         writer.Write(modifiers);
         writer.Write(StringConstants.Space);
         writer.Write(returnType);
         writer.Write(StringConstants.Space);
         writer.Write(name);
      
         if (hasParameters)
         {
            writer.WriteLine(StringConstants.OpenParenthese);
         }
         else
         {
            writer.Write(StringConstants.OpenParenthese);
         }
      }
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodImmediateBuilder CloseHeader(
      this ref MethodImmediateBuilder self, bool semicolon = false)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(!semicolon 
         ? StringConstants.CloseParenthese 
         : StringConstants.CloseParentheseSemicolon);

      if (!semicolon)
      {
         writer.DownIndent();
      }

      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodImmediateBuilder CloseHeaderNoParameters(
      this ref MethodImmediateBuilder self, bool semicolon = false)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(!semicolon 
         ? StringConstants.CloseParenthese 
         : StringConstants.CloseParentheseSemicolon);

      return ref self;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodImmediateBuilder FirstParameter(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> type,
      scoped ReadOnlySpan<char> name)
   {
      ref var writer = ref self.Builder.Writer;
      writer.UpIndent();
      
      var totalSize = type.Length + name.Length + 1;

      if (totalSize <= 1024)
      {
         Span<char> buffer = stackalloc char[totalSize];
         var completeBuffer = buffer;
         
         type.CopyTo(buffer);
         buffer = buffer[type.Length..];
         
         buffer[0] = ' ';
         buffer = buffer[1..];
         
         name.CopyTo(buffer);
         writer.Write(completeBuffer);
      }
      else
      {
         writer.Write(type);
         writer.Write(StringConstants.Space);
         writer.Write(name);
      }
      
      return ref self;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodImmediateBuilder NextParameter(
      this ref MethodImmediateBuilder self,
      scoped ReadOnlySpan<char> type,
      scoped ReadOnlySpan<char> name)
   {
      ref var writer = ref self.Builder.Writer;
      writer.WriteLine(StringConstants.Comma);
      
      writer.Write(type);
      writer.Write(" ");
      writer.Write(name);
      
      return ref self;
   }
}
