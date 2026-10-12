using WJb;

public sealed record EchoPayload(int Id, string Name);

[WJb.ActionName("echo")]
public sealed class WJbEchoAction : JobAction<EchoPayload?>
{
    public static int Completed;

    public static int Expected;

    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public override ValueTask<ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
    {
        var completed = Interlocked.Increment(ref Completed);

        if (completed >= Expected) Completion.TrySetResult();

        return ActionResult.Empty;
    }
}