using System.Runtime.CompilerServices;
using CodeGen.Contracts.Enums;

namespace CodeGen.Writing.Builders.Interfaces;

public interface IAccessBuilder : ICodeBuilder;

public static class AccessBuilderExtensions
{
   extension<T>(ref T builder)
      where T : struct, IAccessBuilder, allows ref struct
   {
      public ref T WriteAccess(AccessModifier modifier, bool indented = true)
      {
         ref var writer = ref builder.GetBuilder().Writer;
         
         var length = modifier.GetCharBufferSize();
         var span = indented ? writer.AcquireSpanIndented(length)
            : writer.AcquireSpan(length);
         
         modifier.FillCharBuffer(span);
         
         return ref builder;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessPublic(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.Public, indented);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessInternal(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.Internal, indented);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessPrivate(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.Private, indented);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessProtected(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.Protected, indented);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessProtectedInternal(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.ProtectedInternal, indented);
      
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public ref T WriteAccessPrivateProtected(bool indented = true) 
         => ref WriteAccess<T>(ref builder, AccessModifier.PrivateProtected, indented);
   }
}