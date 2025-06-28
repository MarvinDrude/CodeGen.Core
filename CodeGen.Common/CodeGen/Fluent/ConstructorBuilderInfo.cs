using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.Buffers.Dynamic;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ConstructorBuilderInfo : IByteSerializable<ConstructorBuilderInfo>
{
   internal AccessModifier AccessModifier;
   internal bool IsStatic;

   internal bool HasBaseCall;
   internal bool HasThisCall;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public ConstructorBuilderInfo(ref ClassBuilderInfo builder, AccessModifier accessModifier)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
      AccessModifier = accessModifier;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref ClassBuilderInfo Done()
   {
      ref var builder = ref ClassBuilder.Builder;
      
      builder.AddTemporaryData(builder.RegionIndexConstructors, in this);
      
      return ref ClassBuilder;
   }
}

public static partial class ConstructorBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo IsStatic(this ref ConstructorBuilderInfo info, bool set = true)
   {
      info.IsStatic = set;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo WithBaseCall(this ref ConstructorBuilderInfo info)
   {
      if (info.HasThisCall)
      {
         throw new InvalidOperationException("Cannot set base call if this call is already set.");
      }
      
      info.HasBaseCall = true;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo WithThisCall(this ref ConstructorBuilderInfo info)
   {
      if (info.HasBaseCall)
      {
         throw new InvalidOperationException("Cannot set this call if base call is already set.");
      }
      
      info.HasThisCall = true;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo AddBaseParameter(this ref ConstructorBuilderInfo info, ReadOnlySpan<char> name)
   {
      if (info.HasThisCall)
      {
         throw new InvalidOperationException("Cannot add base parameter if its already a this call.");
      }

      info.HasBaseCall = true;
      ref var builder = ref info.ClassBuilder.Builder;
      var str = new RefStringView(name);
      
      builder.AddTemporaryData(builder.RegionIndexConstructorParameters, in str);
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo AddThisParameter(this ref ConstructorBuilderInfo info, ReadOnlySpan<char> name)
   {
      if (info.HasBaseCall)
      {
         throw new InvalidOperationException("Cannot add base parameter if its already a this call.");
      }

      info.HasThisCall = true;
      ref var builder = ref info.ClassBuilder.Builder;
      var str = new RefStringView(name);
      
      builder.AddTemporaryData(builder.RegionIndexConstructorParameters, in str);
      return ref info;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo AddParameter(
      this ref ConstructorBuilderInfo info, 
      ReadOnlySpan<char> typeName,
      ReadOnlySpan<char> name,
      ReadOnlySpan<char> attributes = default,
      )
   {
      
   }
}