using WJb;

public sealed class NoOpInput
{
}

public sealed class NoOpAction : JobAction<NoOpInput>
{
    public override Task<IActionResult> ExecuteAsync(NoOpInput input, CancellationToken ct = default)
        => CompleteAsync();
}