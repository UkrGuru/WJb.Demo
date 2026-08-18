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
        cfg.AddAction<HelloAction>("hello");
        cfg.AddAction<ProgressAction>("progress");

        // Configured
        cfg.AddAction<ConfiguredAction>("configured");

        // Chained
        cfg.AddAction<SendEmailAction>("send-email");
        cfg.AddAction<LogAction>("log");
        cfg.AddAction<ErrorLogAction>("error-log");

        // Retry Workflow
        cfg.AddAction<RetryEmailAction>("retry-email");

        // Order Workflow
        cfg.AddAction<CreateOrderAction>("create-order");
        cfg.AddAction<ReserveStockAction>("reserve-stock");
        cfg.AddAction<ChargePaymentAction>("charge-payment");
        cfg.AddAction<SendConfirmationAction>("send-confirmation");

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