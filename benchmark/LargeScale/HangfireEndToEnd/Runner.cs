
using Hangfire;
using Hangfire.MemoryStorage;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        // Reset benchmark state.
        HangfireEchoJob.Completed = 0;
        HangfireEchoJob.Expected = JobCount;
        HangfireEchoJob.Sum = 0;

        HangfireEchoJob.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var storage = new MemoryStorage();

        using (var server = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            storage))
        {
            var client = new BackgroundJobClient(storage);

            // Enqueue all jobs.
            for (int i = 0; i < JobCount; i++)
            {
                client.Enqueue<HangfireEchoJob>(
                    x => x.Execute(
                        new EchoPayload(i, $"User {i}")));
            }

            // Wait until all jobs have executed.
            await HangfireEchoJob.Completion.Task;
        }

        // Verification.
        long expectedSum =
            (long)JobCount * (JobCount - 1) / 2;

        int completed =
            Volatile.Read(ref HangfireEchoJob.Completed);

        long sum =
            Interlocked.Read(ref HangfireEchoJob.Sum);

        Console.WriteLine($"Completed   : {completed:N0}");
        Console.WriteLine($"Expected    : {JobCount:N0}");
        Console.WriteLine($"Sum         : {sum:N0}");
        Console.WriteLine($"ExpectedSum : {expectedSum:N0}");

        if (completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Completed={completed:N0}, Expected={JobCount:N0}");
        }

        if (sum != expectedSum)
        {
            throw new InvalidOperationException(
                $"Sum={sum:N0}, ExpectedSum={expectedSum:N0}");
        }

        Console.WriteLine("Verification: PASSED");
    }
}
