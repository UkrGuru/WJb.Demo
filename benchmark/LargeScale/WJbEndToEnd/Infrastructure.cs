using WJb;

public sealed record EchoPayload(int Id, string Name);

[ActionName("echo")]
public sealed class Echo : JobAction<EchoPayload?>
{
    public static int Completed;
    public static int Expected;

    public static long Sum;

    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public override ValueTask<ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
    {
        if (payload is not null)
        {
            Interlocked.Add(ref Sum, payload.Id);
            _ = payload.Name;
        }

        var completed = Interlocked.Increment(ref Completed);

        if (completed >= Expected)
            Completion.TrySetResult();

        return ActionResult.Empty;
    }
}