using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CodeGen.Common.CodeGen;
using CodeGen.Common.CodeGen.Fluent;

namespace CodeGen.Core.Perfs.CodeGen;

[SimpleJob(RunStrategy.Throughput, iterationCount: 6)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
public class FluentBenchmark
{
   [Benchmark]
   public char SimpleNoAllocationBenchmark()
   {
      var builder = new CodeBuilder(
         stackalloc char[512],
         stackalloc char[128],
         3, ' ');
      builder.SetTemporaryBuffer(stackalloc byte[1024]);

      var test = builder.CreateClass();
      test.WithName("TestA")
         .Render();
      
      var cha = builder.Writer.WrittenSpan[2];
      builder.Dispose();

      return cha;
   }
}