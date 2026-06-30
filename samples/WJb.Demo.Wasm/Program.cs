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

        // Retry Workflow
        cfg.AddAction<RetryEmailAction>();

        // Order Workflow
        cfg.AddAction<CreateOrderAction>();
        cfg.AddAction<ReserveStockAction>();
        cfg.AddAction<ChargePaymentAction>();
        cfg.AddAction<SendConfirmationAction>();

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

