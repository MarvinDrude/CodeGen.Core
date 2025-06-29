using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct GenericBuilderInfo : IByteSerializable<GenericBuilderInfo>
{
   internal RefStringView Name;
   internal int ConstraintLength;
   internal int ConstraintCount;

   internal int Offset;
   internal int ByteLength;

   internal int RegionIndexGenerics;
   internal int RegionIndexConstraints;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }

   public GenericBuilderInfo(
      ref ClassBuilderInfo builder, 
      ReadOnlySpan<char> name,
      int regionIndexGenerics,
      int regionIndexConstraints)
   {
      RegionIndexGenerics = regionIndexGenerics;
      RegionIndexConstraints = regionIndexConstraints;
      
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
      Name = new RefStringView(name);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ref ClassBuilderInfo Done()
   {
      ref var builder = ref ClassBuilder.Builder;
      
      builder.AddTemporaryData(RegionIndexGenerics, in this);
      
      builder.ClearTemporaryData(RegionIndexConstraints);
      return ref ClassBuilder;
   }

   public static void Write(scoped Span<byte> buffer, scoped in GenericBuilderInfo instance)
   {
      ref var builder = ref instance.ClassBuilder.Builder;
      
      var nameLength = RefStringView.CalculateByteLength(in instance.Name);
      RefStringView.Write(buffer, in instance.Name);
      
      buffer = buffer[nameLength..];
      
      BinaryPrimitives.WriteInt32LittleEndian(buffer, instance.ConstraintCount);
      buffer = buffer[sizeof(int)..];
      
      BinaryPrimitives.WriteInt32LittleEndian(buffer, instance.ConstraintLength);
      buffer = buffer[sizeof(int)..];

      foreach (var constraint in builder.GetTemporaryEnumerator<RefStringView>(
                  instance.RegionIndexConstraints))
      {
         var length = RefStringView.CalculateByteLength(in constraint);
         RefStringView.Write(buffer, in constraint);
         
         buffer = buffer[length..];
      }
   }

   public static int Read(ReadOnlySpan<byte> buffer, out GenericBuilderInfo instance)
   {
      instance = new GenericBuilderInfo();

      var nameLength = RefStringView.Read(buffer, out var nameView);
      buffer = buffer[nameLength..];
      
      var constraintCount = BinaryPrimitives.ReadInt32LittleEndian(buffer);
      buffer = buffer[sizeof(int)..];

      var constraintLength = BinaryPrimitives.ReadInt32LittleEndian(buffer);
      buffer = buffer[sizeof(int)..];
      
      instance.Name = nameView;
      instance.ConstraintCount = constraintCount;

      instance.Offset = nameLength + sizeof(int) + sizeof(int);
      instance.ByteLength = constraintLength;
      
      return instance.Offset + instance.ByteLength;
   }

   public static int CalculateByteLength(scoped in GenericBuilderInfo instance)
   {
      return instance.ConstraintLength + sizeof(int) + sizeof(int) + RefStringView.CalculateByteLength(in instance.Name);
   }
}

public static partial class GenericBuilderInfoExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref GenericBuilderInfo AddConstraint(this ref GenericBuilderInfo info, ReadOnlySpan<char> constraint)
   {
      var view = new RefStringView(constraint);
      
      info.ClassBuilder.Builder.AddTemporaryData(info.RegionIndexConstraints, view);
      info.ConstraintLength += RefStringView.CalculateByteLength(in view);
      info.ConstraintCount++;
      
      return ref info;
   }
}