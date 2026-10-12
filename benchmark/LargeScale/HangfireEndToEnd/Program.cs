using System.Diagnostics;

Console.WriteLine("Starting Hangfire benchmark...");
Console.WriteLine();

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

var initialMemory = GC.GetTotalMemory(true);

var sw = Stopwatch.StartNew();

await Runner.ExecuteAsync();

sw.Stop();

var finalMemory = GC.GetTotalMemory(false);

Console.WriteLine();
Console.WriteLine($"Elapsed     : {sw.Elapsed}");
Console.WriteLine($"Jobs/sec    : {(Runner.JobCount / sw.Elapsed.TotalSeconds):N0}");
Console.WriteLine($"Memory      : {(finalMemory - initialMemory) / 1024d / 1024d:N2} MB");