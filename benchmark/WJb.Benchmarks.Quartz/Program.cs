
using BenchmarkDotNet.Running;
using WJb.Benchmarks.Quartz;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
BenchmarkRunner.Run<EnqueueManyBenchmarks>();
