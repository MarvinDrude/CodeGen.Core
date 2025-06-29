using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct MethodBuilderInfo
{
   internal AccessModifier AccessModifier;
   internal MethodModifier Modifiers;
   
   internal RefStringView Name;
   internal RefStringView ReturnType;

   internal bool IsSignatureOnly;
   
   internal int ParameterOffset;
   internal int ParameterCount;
   internal int ParameterLength;
   
   internal int GenericParameterCount;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public MethodBuilderInfo(ref ClassBuilderInfo builder, ReadOnlySpan<char> name)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
      Name = name;

      AccessModifier = AccessModifier.Public;
      Modifiers = MethodModifier.None;
   }
}

public static partial class MethodBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo SetReturnType(this ref MethodBuilderInfo info, ReadOnlySpan<char> type)
   {
      info.ReturnType = type;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo SetName(this ref MethodBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Name = name;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsSignatureOnly(this ref MethodBuilderInfo info, bool set = true)
   {
      info.IsSignatureOnly = set;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo WithAccessModifier(this ref MethodBuilderInfo info, AccessModifier accessModifier)
   {
      info.AccessModifier = accessModifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsPublic(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Public);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsPrivate(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Private);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsProtected(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Protected);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsInternal(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Internal);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsPrivateProtected(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.PrivateProtected);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsProtectedInternal(this ref MethodBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.ProtectedInternal);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo AddModifier(this ref MethodBuilderInfo info, MethodModifier modifier)
   {
      info.Modifiers |= modifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsUnsafe(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Unsafe);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsNew(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.New);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsStatic(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Static);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsVirtual(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Virtual);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsAbstract(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Abstract);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsOverride(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Override);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsSealed(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Sealed);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsExtern(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Extern);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsPartial(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Partial);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsAsync(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Async);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsReadOnly(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.ReadOnly);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo IsRef(this ref MethodBuilderInfo info) => ref info.AddModifier(MethodModifier.Ref);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref MethodBuilderInfo RemoveModifier(this ref MethodBuilderInfo info, MethodModifier modifier)
   {
      info.Modifiers &= ~modifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static GenericBuilderInfo AddGenericParameter(this ref MethodBuilderInfo info, ReadOnlySpan<char> name)
   {
      ref var builder = ref info.ClassBuilder.Builder;
      
      info.GenericParameterCount++;
      return new GenericBuilderInfo(
         ref info.ClassBuilder, name,
         builder.RegionIndexMethodGenerics,
         builder.RegionIndexMethodGenericConstraints);
   }
   
   public static ref MethodBuilderInfo AddParameter(
      this ref MethodBuilderInfo info,
      ReadOnlySpan<char> typeName,
      ReadOnlySpan<char> name,
      ParameterModifier modifiers = ParameterModifier.None,
      ReadOnlySpan<char> attributes = default)
   {
      ref var builder = ref info.ClassBuilder.Builder;
      var parameter = new ParameterBuilderInfo(ref info.ClassBuilder)
      {
         TypeName = typeName,
         Name = name,
         Modifiers = modifiers,
         Attributes = attributes
      };

      info.ParameterCount++;
      info.ParameterLength += ParameterBuilderInfo.CalculateByteLength(in parameter);
      builder.AddTemporaryData(builder.RegionIndexMethodParameters, in parameter);
      return ref info;
   }
}