using Quartz;

public sealed record EchoPayload(int Id, string Name);

[WJb.ActionName("echo")]
public sealed class WJbEchoAction : WJb.JobAction<EchoPayload?>
{
    public static int Completed;

    public override ValueTask<WJb.ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
    {
        _ = payload?.Id;
        _ = payload?.Name;

        Interlocked.Increment(ref Completed);

        return ValueTask.FromResult(WJb.Results.Done());
    }
}

public sealed class HangfireEchoJob
{
    public static int Completed;

    public EchoPayload Execute(EchoPayload payload)
    {
        _ = payload.Id;
        _ = payload.Name;

        Interlocked.Increment(ref Completed);

        return payload;
    }
}

public sealed class QuartzEchoJob : Quartz.IJob
{
    public static int Completed;

    public ValueTask Execute(
        IJobExecutionContext context, CancellationToken ct = default)
    {
        var payload = (EchoPayload)context.MergedJobDataMap["payload"];

        _ = payload.Id;
        _ = payload.Name;

        Interlocked.Increment(ref Completed);

        return ValueTask.CompletedTask;
    }
}