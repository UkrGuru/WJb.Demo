
using Quartz;

public sealed record EchoPayload(int Id, string Name);

public sealed class QuartzEchoJob : IJob
{
    public static int Completed;
    public static int Expected;

    public static long Sum;

    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public ValueTask Execute(
        IJobExecutionContext context,
        CancellationToken ct = default)
    {
        var payload = (EchoPayload?)context.MergedJobDataMap["payload"];

        if (payload is not null)
        {
            Interlocked.Add(ref Sum, payload.Id);

            _ = payload.Name;
        }

        var completed = Interlocked.Increment(ref Completed);

        if (completed >= Expected)
            Completion.TrySetResult();

        return ValueTask.CompletedTask;
    }
}
