using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Contracts.Buffers;
using CodeGen.Writing.Builders.Interfaces;

namespace CodeGen.Writing.Builders.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ClassHeaderBuilder : ICodeBuilder
{
   private readonly ByReferenceStack _builder;
   internal ref CodeBuilder Builder => ref _builder.AsRef<CodeBuilder>();
   
   public ClassHeaderBuilder(ref CodeBuilder builder)
   {
      _builder = ByReferenceStack.Create(ref builder);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   ref CodeBuilder ICodeBuilder.GetBuilder()
   {
      return ref Builder;
   }
}

public static class ClassHeaderBuilderExtensions
{
   extension(ref ClassHeaderBuilder builder)
   {
      
   }
}