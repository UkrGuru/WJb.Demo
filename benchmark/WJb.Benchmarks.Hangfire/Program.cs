
using BenchmarkDotNet.Running;
using WJb.Benchmarks.Hangfire;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
BenchmarkRunner.Run<EnqueueManyBenchmarks>();
