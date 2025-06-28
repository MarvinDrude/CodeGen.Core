using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CodeGen.Common.Buffers;
using CodeGen.Common.Buffers.Dynamic;
using CodeGen.Common.Buffers.Utils;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Common.CodeGen.Fluent;

[StructLayout(LayoutKind.Sequential)]
public ref struct ParameterBuilderInfo : IByteSerializable<ParameterBuilderInfo>
{
   internal RefStringView TypeName;
   internal RefStringView Name;

   internal RefStringView Attributes;
   internal ParameterModifier Modifiers;
   
   private readonly ref byte _builderReference;
   internal ref ClassBuilderInfo ClassBuilder
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ref Unsafe.As<byte, ClassBuilderInfo>(ref _builderReference);
   }
   
   public ParameterBuilderInfo(ref ClassBuilderInfo builder)
   {
      _builderReference = ref Unsafe.As<ClassBuilderInfo, byte>(ref builder);
   }
   
   public static void Write(scoped Span<byte> buffer, scoped in ParameterBuilderInfo instance)
   {
      var nameLength = RefStringView.CalculateByteLength(in instance.Name);
      RefStringView.Write(buffer, in instance.Name);
      buffer = buffer[nameLength..];
      
      var typeLength = RefStringView.CalculateByteLength(in instance.TypeName);
      RefStringView.Write(buffer, in instance.TypeName);
      buffer = buffer[typeLength..];
      
      var attrsLength = RefStringView.CalculateByteLength(in instance.Attributes);
      RefStringView.Write(buffer, in instance.Attributes);
      buffer = buffer[attrsLength..];

      var wrote = instance.Modifiers.WriteLittleEndian(buffer);
      buffer = buffer[wrote..];
   }

   public static int Read(ReadOnlySpan<byte> buffer, out ParameterBuilderInfo instance)
   {
      instance = new ParameterBuilderInfo();

      var read = RefStringView.Read(buffer, out var name);
      buffer = buffer[read..];
      instance.Name = name;
      
      read = RefStringView.Read(buffer, out var typeName);
      buffer = buffer[read..];
      instance.TypeName = typeName;
      
      read = RefStringView.Read(buffer, out var attrs);
      buffer = buffer[read..];
      instance.Attributes = attrs;

      instance.Modifiers = buffer.ReadLittleEndian<ParameterModifier>(out read);
      buffer = buffer[read..];

      return CalculateByteLength(instance);
   }

   public static int CalculateByteLength(scoped in ParameterBuilderInfo instance)
   {
      return instance.Name.ByteLength 
             + instance.TypeName.ByteLength 
             + instance.Attributes.ByteLength 
             + sizeof(ParameterModifier);
   }
}

public static partial class ParameterBuilderInfoExtensions
{
   
}