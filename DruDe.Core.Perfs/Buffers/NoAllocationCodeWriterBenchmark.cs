using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using DruDe.Core.Buffers;

namespace DruDe.Core.Perfs.Buffers;

[SimpleJob(RunStrategy.Throughput, iterationCount: 2)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
public class NoAllocationCodeWriterBenchmark
{
   [Params(1_000, 10_000)]
   public int N;
   
   [Benchmark]
   public char Run()
   {
      Span<char> span = stackalloc char[512];
      Span<char> indent = stackalloc char[128];
      
      for (var i = 0; i < N; i++)
      {
         var writer = new CodeTextWriter(
            span,
            indent,
            3, ' ');
         
         writer.WriteLine("public class Test");
         writer.OpenBody();
         writer.WriteLine("public string StrTest { get; set; }");
         writer.WriteLine();
         writer.WriteLine("public Test()");
         writer.OpenBody();
         writer.WriteLine("StrTest = \"Test\";");
         writer.CloseBody();
         writer.CloseBody();
      }
      
      return ' ';
   }
   
   [Benchmark]
   public char Run2()
   {
      for (var i = 0; i < N; i++)
      {
         var stringWriter = new StringBuilder();

         stringWriter.AppendLine("public class Test");
         stringWriter.AppendLine("{");
         stringWriter.AppendLine("   public string StrTest { get; set; }");
         stringWriter.AppendLine();
         stringWriter.AppendLine("   public Test()");
         stringWriter.AppendLine("   {");
         stringWriter.AppendLine("      StrTest = \"Test\";");
         stringWriter.AppendLine("   }");
         stringWriter.AppendLine("}");
      }
      
      return ' ';
   }
}