using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace CodeGen.Writing.Performance.Tests;

[SimpleJob(RunStrategy.Throughput, iterationCount: 6)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
public class CodeTextWriterTests
{
   [Benchmark]
   public char SimpleAllocationTestWithInterpolate()
   {
      var writer = new CodeTextWriter(
         stackalloc char[512], stackalloc char[128]);

      const int temp = 20;
      writer.OpenBody();
      
      for (var index = 0; index < 20; index++)
      {
         writer.WriteLine("Test");
      }
      
      writer.WriteInterpolated($"a {temp}");
      writer.WriteLine();
      
      writer.CloseBody();
      
      var cha = writer.WrittenSpan[5];
      writer.Dispose();

      return cha;
   }
}