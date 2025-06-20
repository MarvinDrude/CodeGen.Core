using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Immediate;
using CodeGen.Common.CodeGen.Models.Methods;

namespace CodeGen.Common.CodeGen.State;

[StructLayout(LayoutKind.Sequential)]
public ref struct MethodStateBuilder : IStateBuilder
{
   private readonly ref byte _builderReference;
   public ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }

   public RefStringView Modifiers;
   public RefStringView ReturnType;
   public RefStringView Name;

   public bool IsOnlyHeader;
   
   public Span<MethodParameter> Parameters;
   public Span<string> GenericConstraints;

   public MethodStateBuilder(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
      ResetHeader();
   }

   public void RenderHeader(bool reset = true)
   {
      Builder.MethodIm.OpenHeader(Modifiers.Span, ReturnType.Span, Name.Span, Parameters.Length > 0);

      if (Parameters.Length > 0)
      {
         for (var e = 0; e < Parameters.Length; e++)
         {
            ref var parameter = ref Parameters[e];
            
            if (e == 0)
            {
               Builder.MethodIm.FirstParameter(parameter.Type, parameter.Name);
               continue;
            }
            
            Builder.MethodIm.NextParameter(parameter.Type, parameter.Name);
         }
      }

      if (Parameters.Length > 0)
      {
         Builder.MethodIm.CloseHeader(IsOnlyHeader && GenericConstraints.Length == 0);
      }
      else
      {
         Builder.MethodIm.CloseHeaderNoParameters(IsOnlyHeader && GenericConstraints.Length == 0);
      }

      if (GenericConstraints.Length > 0)
      {
         for (var e = 0; e < GenericConstraints.Length; e++)
         {
            ref var genericConstraint = ref GenericConstraints[e];
            var semicolon = e == GenericConstraints.Length - 1 && IsOnlyHeader;
            
            if (e != 0)
            {
               Builder.MethodIm.NextGenericConstraint(genericConstraint, semicolon);
               continue;
            }
            
            Builder.MethodIm.FirstGenericConstraint(genericConstraint, semicolon);
         }

         Builder.Writer.DownIndent();
      }
      
      if(reset) ResetHeader();
   }
   
   private void ResetHeader()
   {
      Modifiers = "";
      ReturnType = "";
      Name = "";
      
      IsOnlyHeader = false;
      Parameters = [];
      GenericConstraints = [];
   }
}

public static class MethodStateBuilderExtensions
{
   
}