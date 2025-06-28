using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.Buffers.Dynamic;
using CodeGen.Common.Buffers.Utils;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ConstructorBuilderInfo : IByteSerializable<ConstructorBuilderInfo>
{
   internal AccessModifier AccessModifier;
   internal bool IsStatic;

   internal bool HasBaseCall;
   internal bool HasThisCall;

   internal int ParameterCount;
   internal int ParameterLength;
   internal int BaseParameterCount;
   internal int BaseParameterLength;

   internal int BaseParameterOffset;
   internal int ParameterOffset;
   
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
      
      builder.ClearTemporaryData(builder.RegionIndexConstructorBaseParameters);
      builder.ClearTemporaryData(builder.RegionIndexConstructorParameters);
      return ref ClassBuilder;
   }

   public static void Write(scoped Span<byte> buffer, scoped in ConstructorBuilderInfo instance)
   {
      ref var builder = ref instance.ClassBuilder.Builder;

      var written = instance.AccessModifier.WriteLittleEndian(buffer);
      buffer = buffer[written..];

      var packed = new PackedBools();
      packed.Set(0, instance.IsStatic);
      packed.Set(1, instance.HasBaseCall);
      packed.Set(2, instance.HasThisCall);

      buffer[0] = packed.RawByte;
      buffer = buffer[1..];

      instance.BaseParameterLength.WriteLittleEndian(buffer);
      buffer = buffer[sizeof(int)..];
      
      instance.ParameterLength.WriteLittleEndian(buffer);
      buffer = buffer[sizeof(int)..];
      
      foreach (var baseParameter in builder.GetTemporaryEnumerator<RefStringView>(
                  builder.RegionIndexConstructorBaseParameters))
      {
         var length = RefStringView.CalculateByteLength(baseParameter);
         RefStringView.Write(buffer, in baseParameter);
         
         buffer = buffer[length..];
      }

      foreach (var parameter in builder.GetTemporaryEnumerator<ParameterBuilderInfo>(
                  builder.RegionIndexConstructorParameters))
      {
         var length = ParameterBuilderInfo.CalculateByteLength(parameter);
         ParameterBuilderInfo.Write(buffer, in parameter);
         
         buffer = buffer[length..];
      }
   }

   public static int Read(ReadOnlySpan<byte> buffer, out ConstructorBuilderInfo instance)
   {
      instance = new ConstructorBuilderInfo
      {
         AccessModifier = buffer.ReadLittleEndian<AccessModifier>(out var read)
      };

      buffer = buffer[read..];
      var packed = new PackedBools(buffer[0]);
      buffer = buffer[1..];

      instance.IsStatic = packed.Get(0);
      instance.HasBaseCall = packed.Get(1);
      instance.HasThisCall = packed.Get(2);

      instance.BaseParameterOffset = read + 1 + sizeof(int) + sizeof(int);
      instance.BaseParameterLength = buffer.ReadLittleEndian<int>(out read);

      instance.ParameterOffset = instance.BaseParameterOffset + instance.BaseParameterLength;
      instance.ParameterLength = buffer.ReadLittleEndian<int>(out read);

      return instance.BaseParameterOffset + instance.BaseParameterLength + instance.ParameterLength;
   }

   public static int CalculateByteLength(scoped in ConstructorBuilderInfo instance)
   {
      return instance.BaseParameterLength 
             + instance.ParameterLength
             + sizeof(int) + sizeof(int)
             + 1 + sizeof(AccessModifier);
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

      info.BaseParameterCount++;
      info.HasBaseCall = true;
      ref var builder = ref info.ClassBuilder.Builder;
      var str = new RefStringView(name);
      
      info.BaseParameterLength += RefStringView.CalculateByteLength(in str);
      builder.AddTemporaryData(builder.RegionIndexConstructorBaseParameters, in str);
      return ref info;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo AddThisParameter(this ref ConstructorBuilderInfo info, ReadOnlySpan<char> name)
   {
      if (info.HasBaseCall)
      {
         throw new InvalidOperationException("Cannot add base parameter if its already a this call.");
      }

      info.BaseParameterCount++;
      info.HasThisCall = true;
      ref var builder = ref info.ClassBuilder.Builder;
      var str = new RefStringView(name);
      
      info.BaseParameterLength += RefStringView.CalculateByteLength(in str);
      builder.AddTemporaryData(builder.RegionIndexConstructorBaseParameters, in str);
      return ref info;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref ConstructorBuilderInfo AddParameter(
      this ref ConstructorBuilderInfo info,
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
      builder.AddTemporaryData(builder.RegionIndexConstructorParameters, in parameter);
      return ref info;
   }
}