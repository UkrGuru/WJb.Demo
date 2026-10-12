public sealed record EchoPayload(int Id, string Name);

public sealed class HangfireEchoJob
{
    public static int Completed;

    public static int Expected;

    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Execute(EchoPayload payload)
    {
        _ = payload.Id;
        _ = payload.Name;

        var completed =
            Interlocked.Increment(ref Completed);

        if (completed >= Expected)
        {
            Completion.TrySetResult();
        }
    }
}
