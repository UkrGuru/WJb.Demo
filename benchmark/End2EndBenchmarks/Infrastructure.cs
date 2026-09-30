using Quartz;

public sealed record EchoPayload(int Id, string Name);

[WJb.ActionName("echo")]
public sealed class WJbEchoAction : WJb.JobAction<EchoPayload?>
{
    public override ValueTask<WJb.ActionResult> ExecuteAsync(
        EchoPayload? payload, CancellationToken ct = default)
        => ValueTask.FromResult(WJb.Results.Done(payload));
}

public sealed class HangfireEchoJob
{
    public static int Completed;

    public EchoPayload Execute(EchoPayload payload)
    {
        Interlocked.Increment(ref Completed);

        return payload;
    }
}

public sealed class QuartzEchoJob : Quartz.IJob
{
    public ValueTask Execute(
        Quartz.IJobExecutionContext context, CancellationToken cancellationToken)
    {
        _ = context.MergedJobDataMap.GetInt("Id");
        _ = context.MergedJobDataMap.GetString("Name");

        return ValueTask.CompletedTask;
    }
}
