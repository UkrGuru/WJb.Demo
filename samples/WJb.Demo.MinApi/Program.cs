using WJb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    var wjb = WJbBuilder.Create(store, cfg =>
    {
        cfg.AddAction<DemoAction>(DemoAction.Key);
    });

    return wjb;
});

builder.Services.AddSingleton<IJobExecutor>(
    sp => sp.GetRequiredService<IWJb>());

builder.Services.AddHostedService<JobWorker>();

var app = builder.Build();

app.MapPost("/jobs", async (IJobExecutor executor) =>
{
    var jobId = await executor.EnqueueAsync(
        DemoAction.Key,
        new DemoPayload
        {
            DelayMs = 5000,
            Text = "Done ✅"
        });

    return Results.Ok(new { jobId });
});

app.MapGet("/jobs", async (IStore store) =>
{
    var jobs = await store.GetJobsAsync();

    return Results.Ok(jobs);
});

app.MapGet("/jobs/{id}", async (string id, IStore store) =>
{
    var job = await store.GetJobAsync(id);

    return job is null ? Results.NotFound() : Results.Ok(job);
});

app.MapDelete("/jobs/{id}", async (string id, IStore store) =>
{
    var ok = await store.DeleteJobAsync(id);

    return ok ? Results.Ok() : Results.NotFound();
});

app.Run();

public sealed class DemoPayload
{
    public int DelayMs { get; set; }

    public string Text { get; set; } = "";
}

public sealed class DemoAction : JobAction<DemoPayload>
{
    public const string Key = "demo";

    public override async Task<ActionResult> ExecuteAsync(
        DemoPayload input,
        CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 10)
        {
            ct.ThrowIfCancellationRequested();

            await Task.Delay(input.DelayMs / 10, ct);

            ReportProgress(
                i,
                $"Progress {i}%");
        }

        return ActionResults.Result(new
        {
            ok = true,
            text = input.Text
        });
    }
}

public sealed class JobWorker(IJobExecutor executor) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => executor.ExecuteLoopAsync(stoppingToken);
}