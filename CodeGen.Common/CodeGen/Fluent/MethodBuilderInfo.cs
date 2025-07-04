using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.Buffers.Dynamic;
using CodeGen.Common.Buffers.Utils;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct MethodBuilderInfo : IByteSerializable<MethodBuilderInfo>
{
   internal AccessModifier AccessModifier;
   internal MethodModifier Modifiers;
   
   internal RefStringView Name;
   internal RefStringView ReturnType;

   internal bool IsSignatureOnly;
   
   internal int ParameterOffset;
   internal int ParameterCount;
   internal int ParameterLength;

   internal int GenericParameterOffset;
   internal int GenericParameterCount;
   internal int GenericParameterLength;
   
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

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref ClassBuilderInfo Done()
   {
      ref var builder = ref ClassBuilder.Builder;
      
      builder.AddTemporaryData(builder.RegionIndexMethods, in this);
      
      builder.ClearTemporaryData(builder.RegionIndexMethodParameters);
      builder.ClearTemporaryData(builder.RegionIndexMethodGenerics);
      return ref ClassBuilder;
   }

   public static void Write(scoped Span<byte> buffer, scoped in MethodBuilderInfo instance)
   {
      ref var builder = ref instance.ClassBuilder.Builder;
      
      var written = instance.AccessModifier.WriteLittleEndian(buffer);
      buffer = buffer[written..];
      
      written = instance.Modifiers.WriteLittleEndian(buffer);
      buffer = buffer[written..];
      
      var packed = new PackedBools();
      packed.Set(0, instance.IsSignatureOnly);
      
      buffer[0] = packed.RawByte;
      buffer = buffer[1..];

      written = RefStringView.CalculateByteLength(in instance.Name);
      RefStringView.Write(buffer, in instance.Name);
      buffer = buffer[written..];
      
      written = RefStringView.CalculateByteLength(in instance.ReturnType);
      RefStringView.Write(buffer, in instance.ReturnType);
      buffer = buffer[written..];

      var totalGenericLength = 0;
      var totalGenericConstraintLength = 0;

      foreach (var generic in builder.GetTemporaryEnumerator<GenericBuilderInfo>(
                  builder.RegionIndexMethodGenerics))
      {
         totalGenericLength += GenericBuilderInfo.CalculateByteLength(in generic);
         totalGenericConstraintLength += generic.ByteLength;
      }
      
      totalGenericLength.WriteLittleEndian(buffer);
      buffer = buffer[sizeof(int)..];
      
      totalGenericConstraintLength.WriteLittleEndian(buffer);
      buffer = buffer[sizeof(int)..];

      instance.ParameterLength.WriteLittleEndian(buffer);
      buffer = buffer[sizeof(int)..];
      
      foreach (var generic in builder.GetTemporaryEnumerator<GenericBuilderInfo>(
                  builder.RegionIndexMethodGenerics))
      {
         var render = generic with
         {
            _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref instance.ClassBuilder),
            RegionIndexConstraints = builder.RegionIndexMethodGenericConstraints,
            RegionIndexGenerics = builder.RegionIndexMethodGenerics,
            ConstraintLength = generic.ByteLength
         };
         
         var length = GenericBuilderInfo.CalculateByteLength(in render);
         GenericBuilderInfo.Write(buffer, in render);
         
         buffer = buffer[length..];
      }
      
      foreach (var parameter in builder.GetTemporaryEnumerator<ParameterBuilderInfo>(
                  builder.RegionIndexMethodParameters))
      {
         var length = ParameterBuilderInfo.CalculateByteLength(in parameter);
         ParameterBuilderInfo.Write(buffer, in parameter);
         
         buffer = buffer[length..];
      }
   }

   public static int Read(ReadOnlySpan<byte> buffer, out MethodBuilderInfo instance)
   {
      instance = new MethodBuilderInfo
      {
         AccessModifier = buffer.ReadLittleEndian<AccessModifier>(out var read)
      };
      
      buffer = buffer[read..];
      instance.Modifiers = buffer.ReadLittleEndian<MethodModifier>(out read);
      buffer = buffer[read..];
      
      var packed = new PackedBools(buffer[0]);
      buffer = buffer[1..];

      instance.IsSignatureOnly = packed.Get(0);

      var nameLength = read = RefStringView.Read(buffer, out instance.Name);
      buffer = buffer[read..];
      
      var returnLength = read = RefStringView.Read(buffer, out instance.ReturnType);
      buffer = buffer[read..];

      var lengthGenerics = buffer.ReadLittleEndian<int>(out read);
      buffer = buffer[read..];
      
      var lengthGenericConstraints = buffer.ReadLittleEndian<int>(out read);
      buffer = buffer[read..];
      
      var lengthParameters = buffer.ReadLittleEndian<int>(out read);
      buffer = buffer[read..];
      
      instance.GenericParameterOffset = 
         sizeof(int) * 3 + 1 + sizeof(AccessModifier) + sizeof(MethodModifier)
         + nameLength + returnLength;
      instance.GenericParameterLength = lengthGenerics;

      instance.ParameterOffset = instance.GenericParameterOffset + instance.GenericParameterLength + lengthGenericConstraints;
      instance.ParameterLength = lengthParameters;
      
      return instance.ParameterOffset + instance.ParameterLength;
   }

   public static int CalculateByteLength(scoped in MethodBuilderInfo instance)
   {
      ref var builder = ref instance.ClassBuilder.Builder;
      var regionMeta = builder.TemporaryData.GetRegionMetadata(builder.RegionIndexMethodGenerics);
      var regionConstraintMeta = builder.TemporaryData.GetRegionMetadata(builder.RegionIndexMethodGenericConstraints);
      
      return sizeof(AccessModifier) + sizeof(MethodModifier) + 1 + sizeof(int) * 3
             + RefStringView.CalculateByteLength(in instance.Name)
             + RefStringView.CalculateByteLength(in instance.ReturnType)
             + instance.ParameterLength + regionMeta.InnerOffset + regionConstraintMeta.InnerOffset;
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
         builder.RegionIndexMethodGenericConstraints,
         false);
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