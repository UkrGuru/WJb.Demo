
using BenchmarkDotNet.Running;
using WJb.Benchmarks.Enqueue;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
//BenchmarkRunner.Run<ParallelBenchmarks>();
