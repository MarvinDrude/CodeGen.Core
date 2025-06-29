using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ClassBuilderInfo
{
   internal RefStringView Name;
   internal RefStringView BaseClassName;

   internal AccessModifier AccessModifier;
   internal ClassModifier ClassModifiers;
   internal ClassType Type;

   internal bool IsHeaderRendered;
   internal int GenericParameterCount;
   internal int ConstructorCount;
   internal int MethodCount;
   
   private readonly ref byte _builderReference;
   internal ref CodeBuilder Builder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, CodeBuilder>(ref _builderReference);
   }
   
   public ClassBuilderInfo(ref CodeBuilder builder)
   {
      _builderReference = ref Unsafe.As<CodeBuilder, byte>(ref builder);
      IsHeaderRendered = false;
      
      AccessModifier = AccessModifier.Public;
      ClassModifiers = ClassModifier.None;
      Type = ClassType.Class;
   }
}

public static partial class ClassBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo SetName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Name = name;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo SetBaseClassName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.BaseClassName = name;
      return ref info;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo AddInterfaceName(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.Builder.AddTemporaryData(info.Builder.RegionIndexClassInterfaces, new RefStringView(name));
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo WithAccessModifier(this ref ClassBuilderInfo info, AccessModifier accessModifier)
   {
      info.AccessModifier = accessModifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsPublic(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Public);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsPrivate(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Private);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsProtected(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Protected);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsInternal(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.Internal);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsPrivateProtected(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.PrivateProtected);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsProtectedInternal(this ref ClassBuilderInfo info) => ref info.WithAccessModifier(AccessModifier.ProtectedInternal);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo AddClassModifier(this ref ClassBuilderInfo info, ClassModifier modifier)
   {
      info.ClassModifiers |= modifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsUnsafe(this ref ClassBuilderInfo info) => ref info.AddClassModifier(ClassModifier.Unsafe);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsStatic(this ref ClassBuilderInfo info) => ref info.AddClassModifier(ClassModifier.Static);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsSealed(this ref ClassBuilderInfo info) => ref info.AddClassModifier(ClassModifier.Sealed);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsPartial(this ref ClassBuilderInfo info) => ref info.AddClassModifier(ClassModifier.Partial);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsAbstract(this ref ClassBuilderInfo info) => ref info.AddClassModifier(ClassModifier.Abstract);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo RemoveClassModifier(this ref ClassBuilderInfo info, ClassModifier modifier)
   {
      info.ClassModifiers &= ~modifier;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo SetClassType(this ref ClassBuilderInfo info, ClassType type)
   {
      info.Type = type;
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsClass(this ref ClassBuilderInfo info) => ref info.SetClassType(ClassType.Class);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsRecordClass(this ref ClassBuilderInfo info) => ref info.SetClassType(ClassType.RecordClass);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsStruct(this ref ClassBuilderInfo info) => ref info.SetClassType(ClassType.Struct);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsRecordStruct(this ref ClassBuilderInfo info) => ref info.SetClassType(ClassType.RecordStruct);
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ClassBuilderInfo IsInterface(this ref ClassBuilderInfo info) => ref info.SetClassType(ClassType.Interface);
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static GenericBuilderInfo AddGenericParameter(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      ref var builder = ref info.Builder;
      
      info.GenericParameterCount++;
      return new GenericBuilderInfo(
         ref info, name, 
         builder.RegionIndexClassGenerics, 
         builder.RegionIndexClassGenericConstraints);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ConstructorBuilderInfo AddConstructor(this ref ClassBuilderInfo info, AccessModifier accessModifier)
   {
      info.ConstructorCount++;
      return new ConstructorBuilderInfo(ref info, accessModifier);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static MethodBuilderInfo AddMethod(this ref ClassBuilderInfo info, ReadOnlySpan<char> name)
   {
      info.MethodCount++;
      return new MethodBuilderInfo(ref info, name);
   }
}