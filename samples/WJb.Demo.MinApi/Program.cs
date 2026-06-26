using WJb;
using WJb.Contracts;
using WJb.Core;
using WJb.Extensions;

var builder = WebApplication.CreateBuilder(args);

// =======================
// ✅ setup
// =======================

// Один инстанс
builder.Services.AddSingleton<InMemoryStore>();

// Разные интерфейсы на один store
builder.Services.AddSingleton<IStore>(sp => sp.GetRequiredService<InMemoryStore>());

builder.Services.AddSingleton<IActionFactory>(sp =>
    WJbConfig.Create(cfg =>
{
cfg.AddAction<DemoAction>();
}));

builder.Services.AddSingleton<IJobExecutor>(sp =>
    new JobExecutor(
        sp.GetRequiredService<IStore>(),
        sp.GetRequiredService<IActionFactory>()));

builder.Services.AddHostedService<JobWorker>();

var app = builder.Build();


// =======================
// ✅ POST /jobs
// =======================

app.MapPost("/jobs", async (IJobExecutor executor) =>
{
var jobId = await executor.EnqueueAsync(
    ActionMapBuilder.GetDefaultKey(typeof(DemoAction)),
    new
{
delayMs = 5000,
text = "Done ✅"
});

return Results.Ok(new { jobId });
});


// =======================
// ✅ GET /jobs
// =======================

app.MapGet("/jobs", async (IStore story) =>
{
var jobs = await story.GetJobsAsync();
return Results.Ok(jobs);
});


// ✅ GET /jobs/{id}
app.MapGet("/jobs/{id}", async (string id, IStore story) =>
{
var job = await story.GetJobAsync(id);
return job is null ? Results.NotFound() : Results.Ok(job);
});


// ✅ DELETE /jobs/{id}
app.MapDelete("/jobs/{id}", async (string id, IStore story) =>
{
var ok = await story.DeleteJobAsync(id);
return ok ? Results.Ok() : Results.NotFound();
});


app.Run();


// =======================
// Demo payload
// =======================

public class DemoPayload
{
    public int DelayMs { get; set; }
    public string Text { get; set; } = "";
}


// =======================
// Demo action
// =======================

public class DemoAction : JobAction<DemoPayload>, IProgressAction
{
    public event Action<JobProgress>? OnProgress;

    public override async Task<ActionResult> ExecuteAsync(
        DemoPayload input,
        CancellationToken ct = default)
    {
        for (int i = 0; i <= 100; i += 10)
        {
            ct.ThrowIfCancellationRequested();

            await Task.Delay(input.DelayMs / 10, ct);

            OnProgress?.Invoke(new JobProgress
            {
                Progress = i,
                Message = $"Progress {i}%"
            });
        }

        return ActionResults.Result(new
        {
            ok = true,
            text = input.Text
        });
    }
}


// =======================
// Worker
// =======================

public class JobWorker(IJobExecutor executor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await executor.ExecuteLoopAsync(stoppingToken);
    }
}

