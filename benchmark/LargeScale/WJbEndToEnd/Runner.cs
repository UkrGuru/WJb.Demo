using WJb;

public static class Runner
{
    public const int JobCount = 1_000_000;

    public static async Task ExecuteAsync()
    {
        WJbEchoAction.Completed = 0;
        WJbEchoAction.Expected = JobCount;

        WJbEchoAction.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var store = new InMemoryStore();

        var wjb = WJbBuilder.Create(store, cfg =>
        {
            cfg.AddAction<WJbEchoAction>("echo");
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
            await wjb.EnqueueAsync("echo",
                new EchoPayload(i, $"User {i}"));
        }

        await WJbEchoAction.Completion.Task;

        cts.Cancel();

        try { await worker; }
        catch (OperationCanceledException) { }

        Console.WriteLine($"Completed   : {WJbEchoAction.Completed:N0}");
    }
}