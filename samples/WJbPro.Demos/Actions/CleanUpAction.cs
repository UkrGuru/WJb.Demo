using WJb;

namespace WJbPro.Demos.Actions;

public sealed class CleanUpPayload
{
    public int KeepHours { get; set; } = 1;
}

public sealed class CleanUpAction(IStore store) : JobAction<CleanUpPayload>
{
    public const string Key = "clean-up";

    public override async Task<IActionResult> ExecuteAsync(
        CleanUpPayload input, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(store);

        var before = DateTime.UtcNow.AddHours(-input.KeepHours);

        var removed = 0;

        foreach (var job in await store.GetJobsAsync(ct: ct))
        {
            if (job.RunAt <= before)
            {
                await store.DeleteJobAsync(job.Id, ct);
                removed++;
            }
        }

        ReportProgress(100, $"Removed {removed} jobs.");

        return await CompleteAsync();
    }
}