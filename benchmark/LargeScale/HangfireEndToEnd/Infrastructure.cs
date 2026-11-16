
public sealed record EchoPayload(int Id, string Name);

public sealed class HangfireEchoJob
{
    public static int Completed;

    public static int Expected;

    public static long Sum;

    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Execute(EchoPayload? payload)
    {
        if (payload is not null)
        {
            Interlocked.Add(ref Sum, payload.Id);

            _ = payload.Name;
        }

        var completed = Interlocked.Increment(ref Completed);

        if (completed >= Expected)
            Completion.TrySetResult();
    }
}
