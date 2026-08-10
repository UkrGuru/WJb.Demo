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

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    var wjb = WJbBuilder.Create(store, cfg =>
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

        cfg.AddService(new SmtpSettings
        {
            Host = "smtp.local"
        });
    });

    return wjb;
});

builder.Services.AddSingleton<WasmWorker>();

var app = builder.Build();

app.Services.GetRequiredService<WasmWorker>().Start();

await app.RunAsync();