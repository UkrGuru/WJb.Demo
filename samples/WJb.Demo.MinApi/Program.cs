using WJb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    return WJbBuilder.Create(store, cfg =>
    {
        cfg.AddAction<DemoAction>(DemoAction.Key);
    });
});

builder.Services.AddSingleton(sp => new WasmWorker(sp.GetRequiredService<IWJb>()));

builder.Services.AddSingleton<IJobExecutor>(sp => sp.GetRequiredService<IWJb>());

var app = builder.Build();

app.MapPost("/jobs", async Task<IResult> (IJobExecutor executor) =>
    {
        var jobId = await executor.EnqueueAsync(
            DemoAction.Key,
            new DemoPayload
            {
                DelayMs = 5000,
                Text = "Done ✅"
            });

        return TypedResults.Ok(new { jobId });
    });

app.MapGet("/jobs", async Task<IResult> (IStore store) =>
    {
        var jobs = await store.GetJobsAsync();

        return TypedResults.Ok(jobs);
    });

app.MapGet("/jobs/{id}", async Task<IResult> (string id, IStore store) =>
    {
        var job = await store.GetJobAsync(id);

        return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
    });

app.MapDelete("/jobs/{id}", async Task<IResult> (string id, IStore store) =>
    {
        var ok = await store.DeleteJobAsync(id);

        return ok ? TypedResults.Ok() : TypedResults.NotFound();
    });

app.Services.GetRequiredService<WasmWorker>().Start();

app.Run();

public sealed class DemoPayload
{
    public int DelayMs { get; set; }

    public string Text { get; set; } = "";
}

public sealed class DemoAction : JobAction<DemoPayload>
{
    public const string Key = "demo";

    public override async Task<IActionResult> ExecuteAsync(
        DemoPayload input, CancellationToken ct)
    {
        for (var i = 0; i <= 100; i += 10)
        {
            ct.ThrowIfCancellationRequested();

            await Task.Delay(input.DelayMs / 10, ct);

            ReportProgress(
                i,
                $"Progress {i}%");
        }

        return WJb.Results.Complete(new
        {
            ok = true,
            text = input.Text
        });
    }
}

public sealed class JobWorker(IJobExecutor executor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await executor.ExecuteLoopAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}