
using BenchmarkDotNet.Running;
using DruDe.Core.Perfs.Buffers;

BenchmarkRunner.Run<NoAllocationCodeWriterBenchmark>();

// var test = new NoAllocationCodeWriterBenchmark();
// test.N = 100;
// test.Run();