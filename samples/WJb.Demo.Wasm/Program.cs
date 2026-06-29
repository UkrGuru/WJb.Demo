using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb;
using WJb.Contracts;
using WJb.Core;
using WJb.Demo.Wasm;
using WJb.Demo.Wasm.Actions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<IStore, InMemoryStore>();
builder.Services.AddSingleton<IActionFactory>(_ =>
{
    return WJbConfig.Create(cfg =>
    {
        // Basic demos
        cfg.AddAction<HelloAction>();
        cfg.AddAction<ProgressAction>();

        // Configured
        cfg.AddAction<ConfiguredAction>();

        // Chained
        cfg.AddAction<SendEmailAction>();
        cfg.AddAction<LogAction>();
        cfg.AddAction<ErrorLogAction>();

        // Settings
        cfg.AddSetting(new SmtpSettings
        {
            Host = "smtp.local"
        });
    });
});

builder.Services.AddSingleton<ActionFactory>(sp =>
    (ActionFactory)sp.GetRequiredService<IActionFactory>());

builder.Services.AddSingleton<JobExecutor>();
builder.Services.AddSingleton<JobEngineLite>();

builder.Services.AddSingleton<IJobClient>(sp => sp.GetRequiredService<JobEngineLite>());
builder.Services.AddSingleton<IJobQuery>(sp => sp.GetRequiredService<JobEngineLite>());
builder.Services.AddSingleton<IJobNotifier>(sp => sp.GetRequiredService<JobEngineLite>());

var app = builder.Build();

var engine = app.Services.GetRequiredService<JobEngineLite>();

_ = Task.Run(() => engine.RunAsync());
_ = Task.Run(() => engine.RunAsync());
_ = Task.Run(() => engine.RunAsync());


await app.RunAsync();

public sealed class DemoPayload
{
    public int DelayMs { get; set; } = 1000;

    public string? Text { get; set; }
}

public sealed class DemoAction : JobAction<DemoPayload>
{
    public override async Task<ActionResult> ExecuteAsync(
        DemoPayload input,
        CancellationToken ct = default)
    {
        var totalDelay = Math.Max(input.DelayMs, 1);
        var stepDelay = totalDelay / 10;

        for (int i = 0; i <= 100; i += 10)
        {
            ct.ThrowIfCancellationRequested();

            ReportProgress(i, $"Progress {i}%");

            await Task.Delay(stepDelay, ct);
        }

        return ActionResults.Result(new
        {
            message = input.Text,
            completedAt = DateTime.UtcNow
        });
    }
}