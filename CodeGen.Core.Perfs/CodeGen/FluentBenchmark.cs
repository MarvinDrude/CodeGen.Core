using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CodeGen.Common.CodeGen;
using CodeGen.Common.CodeGen.Fluent;
using CodeGen.Common.CodeGen.Models.Common;

namespace CodeGen.Core.Perfs.CodeGen;

[SimpleJob(RunStrategy.Throughput, iterationCount: 6)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
public class FluentBenchmark
{
   [Benchmark]
   public char SimpleNoAllocationBenchmark()
   {
      // this example does not use heap allocations besides the ToString at the end if needed
      // this setup is probably already pushing the safe limit of sack allocations
      // general rule of thumb can be 1kb to 4kb at max depending on nesting etc.
      var builder = new CodeBuilder(
         stackalloc char[1024],
         stackalloc char[128],
         3, ' ');
      builder.SetTemporaryBuffer(stackalloc byte[1024]);

      // due to the limitation of possible zero heap allocation + the temporary buffer
      // u will always need to create classes one at a time per CodeBuilder until u flush with Render()
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
      
      // Add generic parameters to class if needed
      var genericParamOne = test.AddGenericParameter("TParameter");
      genericParamOne
         .AddConstraint("notnull")
         .Done(); // write it to the current class generic temporary buffer

      var genericParamTwo = test.AddGenericParameter("TResult");
      genericParamTwo
         .AddConstraint("struct")
         .AddConstraint("allows ref struct")
         .Done(); 

      test.Render(); 
      // flush class header to free temporary buffer (stack allocated is limited)
      // u can always choose to not flush and the temporary buffer will be heap allocated automatically
      // as soon as the stack allocated buffer is exceeded

      builder.Writer.WriteLine("private static readonly string StaticFieldTest;");
      builder.Writer.WriteLine();

      builder.Writer.WriteLine("private int _numberField;");
      builder.Writer.WriteLine();
      // you can write normal lines and text between Render() flushes whenever you need custom code
      
      var staticConstructor = test.AddConstructor(AccessModifier.None);
      staticConstructor
         .IsStatic()
         .Done(); // write it to the current class generic temporary buffer
      test.Render(); // flush the static constructor header

      builder.Writer.WriteLine("StaticFieldTest = \"Look a string!\";");
      builder.Writer.CloseBody();
      
      var instanceConstructor = test.AddConstructor(AccessModifier.Public);
      instanceConstructor
         .WithThisCall()
         .AddThisParameter("number") // add one parameter to : this() call
         .Done(); 
      test.Render();
      builder.Writer.CloseBody();

      var instanceConstructorImpl = test.AddConstructor(AccessModifier.Public);
      
      
      // WrittenSpan is full class as ReadOnlySpan<char> if you can work with that and has no additional heap allocation
      var cha = builder.Writer.WrittenSpan[0];
      // ToString allocates a new heap string based on WrittenSpan
      var str = builder.ToString();
      
      // Dispose should always be called (best in a finally) but is only responsible for freeing any heap allocations
      // that were necessary (in this example there were none)
      builder.Dispose();

      return cha;
   }
}