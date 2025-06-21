
using BenchmarkDotNet.Running;
using CodeGen.Common.Buffers;
using CodeGen.Common.CodeGen;
using CodeGen.Common.CodeGen.Immediate;
using CodeGen.Common.CodeGen.Models.Methods;
using CodeGen.Core.Perfs.CodeGen;

// for (var e = 0; e < 10_000; e++)
// {
//    new OldWriterCompareBenchmark().CodeGenHelpersBuilder();
// }

#if RELEASE
BenchmarkRunner.Run<OldWriterCompareBenchmark>();
#else

var builder = new CodeBuilder(
   stackalloc char[12],
   stackalloc char[128],
   3, ' ');

builder.Method.IsOnlyHeader = false;
builder.Method.Name = "AbstractMethod<T, E>";
builder.Method.ReturnType = "void";
builder.Method.Modifiers = "public abstract";
builder.Method.GenericConstraints = [
   "where T : notnull",
   "where E : IInterface",
];
builder.Method.Parameters = [
   new MethodParameter()
   {
      Name = "test",
      Type = "string",
   },
   new MethodParameter()
   {
      Name = "test1",
      Type = "string",
   }
];
builder.Method.RenderHeader();
builder.Writer.OpenBody();
builder.Writer.CloseBody();

var str = builder.ToString();
_ = "";

#endif
