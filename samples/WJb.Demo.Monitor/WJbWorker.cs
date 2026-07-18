namespace WJb.Demo.Monitor;

public sealed class WJbWorker(IWJbExecutor executor): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await executor.ExecuteOnceAsync(stoppingToken);
        }
    }
}