using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Contracts.Buffers;
using CodeGen.Contracts.Enums;
using CodeGen.Writing.Builders.Interfaces;

namespace CodeGen.Writing.Builders.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct TypeHeaderBuilder : IAccessBuilder, IGenericBuilder
{
   private readonly ByReferenceStack _builder;
   internal ref CodeBuilder Builder => ref _builder.AsRef<CodeBuilder>();
   
   public TypeHeaderBuilder(ref CodeBuilder builder)
   {
      _builder = ByReferenceStack.Create(ref builder);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   ref CodeBuilder ICodeBuilder.GetBuilder()
   {
      return ref Builder;
   }
}

public static class TypeHeaderBuilderExtensions
{
   extension(ref TypeHeaderBuilder builder)
   {
      public ref TypeHeaderBuilder WriteStruct(string name, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;
         
         writer.WriteInterpolated($"{(addSpaceInFront ? " " : string.Empty)}struct {name}");
         
         return ref builder;
      }
      
      public ref TypeHeaderBuilder WriteInterface(string name, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;
         
         writer.WriteInterpolated($"{(addSpaceInFront ? " " : string.Empty)}interface {name}");
         
         return ref builder;
      }
      
      public ref TypeHeaderBuilder WriteClass(string name, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;
         
         writer.WriteInterpolated($"{(addSpaceInFront ? " " : string.Empty)}class {name}");
         
         return ref builder;
      }
      
      public ref TypeHeaderBuilder WriteClassModifiers(
         ClassModifier modifiers, bool indented = false, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;

         var length = modifiers.GetCharBufferSize() + (addSpaceInFront ? 1 : 0);
         var span = indented ? writer.AcquireSpanIndented(length)
            : writer.AcquireSpan(length);

         span[0] = ' ';
         modifiers.FillCharBuffer(span[1..]);
         
         return ref builder;
      }
      
      public ref TypeHeaderBuilder WriteInterfaceModifiers(
         InterfaceModifier modifiers, bool indented = false, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;

         var length = modifiers.GetCharBufferSize() + (addSpaceInFront ? 1 : 0);
         var span = indented ? writer.AcquireSpanIndented(length)
            : writer.AcquireSpan(length);

         span[0] = ' ';
         modifiers.FillCharBuffer(span[1..]);
         
         return ref builder;
      }
      
      public ref TypeHeaderBuilder WriteStructModifiers(
         StructModifier modifiers, bool indented = false, bool addSpaceInFront = true)
      {
         ref var writer = ref builder.Builder.Writer;

         var length = modifiers.GetCharBufferSize() + (addSpaceInFront ? 1 : 0);
         var span = indented ? writer.AcquireSpanIndented(length)
            : writer.AcquireSpan(length);

         span[0] = ' ';
         modifiers.FillCharBuffer(span[1..]);
         
         return ref builder;
      }

   }
}