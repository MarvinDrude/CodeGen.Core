﻿using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen;
using CodeGen.Common.CodeGen.Immediate;
using CodeGen.Core.Perfs.Config;

namespace CodeGen.Core.Perfs.Buffers;

[SimpleJob(RunStrategy.Throughput, iterationCount: 2), Config(typeof(BenchmarkConfig))]
[MinColumn, MaxColumn, MeanColumn, MedianColumn, MemoryDiagnoser]
public class NoAllocationCodeWriterBenchmark
{
   [Params(1_000, 10_000)]
   public int N;
   
   [Benchmark]
   public string Run()
   {
      Span<char> span = stackalloc char[512];
      Span<char> indent = stackalloc char[128];
      var result = string.Empty;
      
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

         result = writer.ToString();
         writer.Dispose();
      }
      
      return result;
   }
   
   [Benchmark]
   public char RunNamespace()
   {
      Span<char> span = stackalloc char[512];
      Span<char> indent = stackalloc char[128];
      
      for (var i = 0; i < N; i++)
      {
         var builder = new CodeBuilder(
            span,
            indent,
            3, ' ');
         
         builder.NameSpaceIm
            .EnableNullable()
            .AutoGeneratedComment()
            .WriteLine()
               .Using("System.Test")
               .Using("System.A")
               .Using("System.B")
               .WriteLine()
            .Set("My.NameSpace")
            .WriteLine();

         builder.ClassIm
            .OpenHeader("public static class Test<T, E>")
               .FirstBaseDeclaration("BaseClassOne")
               .NextBaseDeclaration("IInterfaceTwo")
               .CloseBaseDeclaration()
               .FirstGenericConstraint("where T : notnull")
               .NextGenericConstraint("where E : struct, new()")
            .CloseGenericConstraint()
            .CloseHeader()
               .WriteLine("Wwww")
            .CloseBody();

         builder.Writer.WriteLine();
         builder.Writer.WriteLine();
      
         builder.ClassIm
            .OpenHeader("public static class Test<T, E>")
               .FirstGenericConstraint("where T : notnull")
               .NextGenericConstraint("where E : struct, new()")
               .CloseGenericConstraint()
            .CloseHeader()
               .WriteLine("Wwww")
            .CloseBody();
         
         builder.Dispose();
      }
      
      return ' ';
   }
   
   [Benchmark]
   public string Run2()
   {
      var result = string.Empty;
      
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
         
         result = stringWriter.ToString();
      }
      
      return result;
   }
}