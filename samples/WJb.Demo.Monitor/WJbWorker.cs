using WJb;

namespace WJb.Demo.Monitor;

public sealed class WJbWorker(
    IWJbExecutor executor)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        Console.WriteLine("WJbWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await executor.ExecuteOnceAsync(stoppingToken);

            Console.WriteLine($"ExecuteOnceAsync: {result}");

            await Task.Delay(1000, stoppingToken);
        }
    }
}