
using BenchmarkDotNet.Running;
using WJb.Benchmarks.Core;
using WJb.Benchmarks.Dequeue;
using WJb.Benchmarks.Enqueue;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
//BenchmarkRunner.Run<QueueRoundTripBenchmarks>();
