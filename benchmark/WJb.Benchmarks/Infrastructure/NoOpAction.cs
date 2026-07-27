namespace WJb.Benchmarks.Infrastructure;

public sealed class NoOpAction : IAction
{
    public Task<ActionResult> ExecuteAsync(object? input, CancellationToken ct = default)
        => Task.FromResult(ActionResults.None());
}