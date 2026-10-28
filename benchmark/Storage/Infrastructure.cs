using WJb;

public sealed record EchoPayload(int Id, string Name);

[ActionName("echo")]
public sealed class WJbEchoAction : JobAction<EchoPayload?>
{
    public static int Completed;

    public override ValueTask<ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
    {
        _ = payload?.Id;
        _ = payload?.Name;

        Interlocked.Increment(ref Completed);

        return ActionResult.Empty;
    }
}

