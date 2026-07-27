using Quartz;

namespace WJb.Benchmarks.Infrastructure;

public sealed class NoOpJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.CompletedTask;
    }
}