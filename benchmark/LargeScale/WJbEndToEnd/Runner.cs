using WJb;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        Echo.Completed = 0;
        Echo.Expected = JobCount;
        Echo.Sum = 0;

        Echo.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<Echo>("echo");
        });

        using var cts = new CancellationTokenSource();

        var worker = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await wjb.ExecuteLoopAsync();

                await Task.Delay(1, cts.Token);
            }
        });

        for (int i = 0; i < JobCount; i++)
        {
            await wjb.EnqueueAsync("echo", new EchoPayload(i, $"User {i}"));
        }

        await Echo.Completion.Task;

        cts.Cancel();

        try
        {
            await worker;
        }
        catch (OperationCanceledException)
        {
        }

        var expectedSum =
            (long)JobCount * (JobCount - 1) / 2;

        Console.WriteLine($"Completed   : {Echo.Completed:N0}");
        Console.WriteLine($"Expected    : {JobCount:N0}");
        Console.WriteLine($"Sum         : {Echo.Sum:N0}");
        Console.WriteLine($"ExpectedSum : {expectedSum:N0}");

        if (Echo.Completed != JobCount)
        {
            throw new InvalidOperationException(
                $"Completed={Echo.Completed:N0}, Expected={JobCount:N0}");
        }

        if (Echo.Sum != expectedSum)
        {
            throw new InvalidOperationException(
                $"Sum={Echo.Sum:N0}, ExpectedSum={expectedSum:N0}");
        }

        Console.WriteLine("Verification: PASSED");
    }
}