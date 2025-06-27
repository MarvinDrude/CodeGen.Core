using System.Runtime.CompilerServices;
using CodeGen.Common.Buffers.Dynamic;

namespace CodeGen.Common.CodeGen;

public ref partial struct CodeBuilder
{
   private void EnsureTemporaryRegions()
   {
      RegionIndexClassInterfaces = TemporaryData.AddRegion(0);
      RegionIndexClassGenerics = TemporaryData.AddRegion(0);
      RegionIndexClassGenericConstraints = TemporaryData.AddRegion(0);

      RegionIndexConstructors = TemporaryData.AddRegion(0);
      RegionIndexConstructorParameters = TemporaryData.AddRegion(0);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddTemporaryData<T>(int index, scoped in T data)
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      TemporaryData.AddToRegion(index, in data);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void ClearTemporaryData(int index)
   {
      TemporaryData.ClearRegion(index, 0);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RegionedSpan.RegionEnumeratorHolder<T> GetTemporaryEnumerator<T>(int index)
      where T : struct, IByteSerializable<T>, allows ref struct
   {
      return TemporaryData.GetRegionEnumerator<T>(index);
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<byte> GetTemporarySpan(int index)
   {
      return TemporaryData.GetRegion(index);
   }
}