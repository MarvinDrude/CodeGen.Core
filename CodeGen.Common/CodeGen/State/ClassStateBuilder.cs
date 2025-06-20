using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Immediate;

namespace CodeGen.Common.CodeGen.State;

[StructLayout(LayoutKind.Sequential)]
public ref struct ClassStateBuilder : IStateBuilder
{
   private readonly ref byte _builderReference;
   public ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }

   public RefStringView Declaration;
   public Span<string> BaseDeclarations;
   public Span<string> GenericConstraints;

   public ClassStateBuilder(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
      ResetDeclaration();
   }

   public void RenderDeclaration(bool reset = true)
   {
      Builder.ClassIm.OpenHeader(Declaration.Span);

      if (BaseDeclarations.Length > 0)
      {
         for (var e = 0; e < BaseDeclarations.Length; e++)
         {
            ref var baseDeclaration = ref BaseDeclarations[e];
            
            if (e == 0)
            {
               Builder.ClassIm.FirstBaseDeclaration(baseDeclaration);
               continue;
            }

            Builder.ClassIm.NextBaseDeclaration(baseDeclaration);
         }

         Builder.ClassIm.CloseBaseDeclaration();
      }

      if (GenericConstraints.Length > 0)
      {
         for (var e = 0; e < GenericConstraints.Length; e++)
         {
            ref var genericConstraint = ref GenericConstraints[e];

            if (e == 0)
            {
               Builder.ClassIm.FirstGenericConstraint(genericConstraint);
               continue;
            }

            Builder.ClassIm.NextGenericConstraint(genericConstraint);
         }

         Builder.ClassIm.CloseGenericConstraint();
      }

      Builder.ClassIm.CloseHeader();
      
      if(reset) ResetDeclaration();
   }
   
   private void ResetDeclaration()
   {
      Declaration = "public class Test";
      BaseDeclarations = [];
      GenericConstraints = [];
   }
}

public static class ClassStateBuilderExtensions
{
   
}