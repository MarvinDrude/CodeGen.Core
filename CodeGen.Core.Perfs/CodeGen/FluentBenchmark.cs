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
      test
         .SetName("TestA")
            .IsRecordClass()
            .IsPartial()
            .IsInternal()
            .IsUnsafe()
         .SetBaseClassName("TestBase")
             .AddInterfaceName("IInterfaceTwo")
             .AddInterfaceName("IInterfaceThree");

       var genericParamOne = test.AddGenericParameter("TParameter");
       genericParamOne
          .AddConstraint("notnull")
          .AddConstraint("ISuperInterface")
          .Done();
      
       var genericParamTwo = test.AddGenericParameter("TResult");
       genericParamTwo
          .AddConstraint("struct")
          .AddConstraint("allows ref struct")
          .Done();

      test.Render();
      
      
      
      var cha = builder.Writer.WrittenSpan[0];
      var str = builder.ToString();
      builder.Dispose();

      return cha;
   }
}