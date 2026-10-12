using Hangfire;
using Hangfire.MemoryStorage;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        HangfireEchoJob.Completed = 0;
        HangfireEchoJob.Expected = JobCount;

        HangfireEchoJob.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var storage = new MemoryStorage();

        using var server = new BackgroundJobServer(
            new BackgroundJobServerOptions
            {
                WorkerCount = 1
            },
            storage);

        var client = new BackgroundJobClient(storage);

        for (int i = 0; i < JobCount; i++)
        {
            client.Enqueue<HangfireEchoJob>(
                x => x.Execute(
                    new EchoPayload(i, $"User {i}")));
        }

        await HangfireEchoJob.Completion.Task;

        if (HangfireEchoJob.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Expected={JobCount}, Actual={HangfireEchoJob.Completed}");
        }

        Console.WriteLine(
            $"Completed   : {HangfireEchoJob.Completed:N0}");
    }
}