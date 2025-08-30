using System.Runtime.InteropServices;
using CodeGen.Contracts.Buffers;

namespace CodeGen.Writing.Builders.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ClassHeaderBuilder
{
   private readonly ByReferenceStack _builder;
   internal ref CodeBuilder Builder => ref _builder.AsRef<CodeBuilder>();
   
   public ClassHeaderBuilder(ref CodeBuilder builder)
   {
      _builder = ByReferenceStack.Create(ref builder);
   }
}

