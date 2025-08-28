
using BenchmarkDotNet.Running;
using CodeGen.Writing.Performance.Tests;

#if DEBUG 
new CodeTextWriterTests().SimpleAllocationTestWithInterpolate();
#else
BenchmarkRunner.Run<CodeTextWriterTests>();
#endif