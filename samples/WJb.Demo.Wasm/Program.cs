using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb;
using WJb.Demo.Wasm;
using WJb.Demo.Wasm.Actions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddSingleton<IWJb>(_ =>
    WJbBuilder.Create(cfg =>
    {

        // Basic demos
        cfg.AddAction<HelloAction>(HelloAction.Key);
        cfg.AddAction<ProgressAction>(ProgressAction.Key);

        // Configured
        cfg.AddAction<ConfiguredAction>(ConfiguredAction.Key);

        // Chained
        cfg.AddAction<SendEmailAction>(SendEmailAction.Key);
        cfg.AddAction<LogAction>(LogAction.Key);
        cfg.AddAction<ErrorLogAction>(ErrorLogAction.Key);

        // Retry Workflow
        cfg.AddAction<RetryEmailAction>(RetryEmailAction.Key);

        // Order Workflow
        cfg.AddAction<CreateOrderAction>(CreateOrderAction.Key);
        cfg.AddAction<ReserveStockAction>(ReserveStockAction.Key);
        cfg.AddAction<ChargePaymentAction>(ChargePaymentAction.Key);
        cfg.AddAction<SendConfirmationAction>(SendConfirmationAction.Key);


        // Demo
        cfg.AddAction<DemoAction>(DemoAction.Key);

        cfg.AddSetting(new SmtpSettings
        {
            Host = "smtp.local"
        });

        cfg.UseStore(new InMemoryStore());
    }));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IWJb>().Executor!);

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IWJb>().Store!);

builder.Services.AddSingleton<JobEngineLite>();

builder.Services.AddSingleton<IJobClient>(sp =>
    sp.GetRequiredService<JobEngineLite>());

builder.Services.AddSingleton<IJobQuery>(sp =>
    sp.GetRequiredService<JobEngineLite>());

builder.Services.AddSingleton<IJobNotifier>(sp =>
    sp.GetRequiredService<JobEngineLite>());

var app = builder.Build();

var engine = app.Services.GetRequiredService<JobEngineLite>();

for (var i = 0; i < 3; i++)
{
    _ = Task.Run(() => engine.RunAsync());
}

await app.RunAsync();


public sealed class DemoPayload
{
    public int DelayMs { get; set; } = 1000;

    public string? Text { get; set; }
}

public sealed class DemoAction :
    JobAction<DemoPayload>,
    IProgressAction
{
    public const string Key = "demo";

    public event Action<JobProgress>? OnProgress;

    public override async Task<ActionResult> ExecuteAsync(
        DemoPayload input,
        CancellationToken ct)
    {
        var totalDelay = Math.Max(input.DelayMs, 1);
        var stepDelay = Math.Max(totalDelay / 10, 1);

        for (var i = 0; i <= 100; i += 10)
        {
            ct.ThrowIfCancellationRequested();

            OnProgress?.Invoke(new JobProgress
            {
                Progress = i,
                Message = $"Progress {i}%"
            });

            await Task.Delay(stepDelay, ct);
        }

        return ActionResults.Result(new
        {
            message = input.Text,
            completedAt = DateTime.UtcNow
        });
    }
}