public sealed class NoOpJob
{
    public Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}