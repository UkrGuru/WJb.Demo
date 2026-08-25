using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb;
using WJb.Demo.Wasm;
using WJbPro.Demos.Actions;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<HttpClient>(_ =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddSingleton<IStore, InMemoryStore>();

//builder.Services.AddSingleton<IWJb>(sp =>
//{
//    var store = sp.GetRequiredService<IStore>();

//    var wjb = WJbBuilder.Create(store, cfg =>
//    {
//        // Basic demos
//        cfg.AddAction<HelloAction>(HelloAction.Key);
//        cfg.AddAction<ProgressAction>(ProgressAction.Key);

//        // Retry Workflow
//        cfg.AddAction<RetryEmailAction>(RetryEmailAction.Key);
//        cfg.AddAction<LogAction>(LogAction.Key);

//        // Order Workflow
//        cfg.AddAction<CreateOrderAction>(CreateOrderAction.Key);
//        cfg.AddAction<ReserveStockAction>(ReserveStockAction.Key);
//        cfg.AddAction<ChargePaymentAction>(ChargePaymentAction.Key);
//        cfg.AddAction<SendConfirmationAction>(SendConfirmationAction.Key);

//        // Configured
//        cfg.AddAction<ConfiguredAction>(ConfiguredAction.Key);
//        cfg.AddService(new SmtpSettings
//        {
//            Host = "smtp.local"
//        });

//        // Http Ping
//        cfg.AddAction<HttpPingAction>(HttpPingAction.Key, new { cron = "* * * * *" });

//        cfg.AddAction<ImportCustomersAction>(ImportCustomersAction.Key);
//        cfg.AddAction<GenerateReportAction>(GenerateReportAction.Key);
//        cfg.AddAction<SendEmailAction>(SendEmailAction.Key);
//    });

//    return wjb;
//}); 

var http = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

var actionsJson = await http.GetStringAsync("App_Data/actions.json");
var servicesJson = await http.GetStringAsync("App_Data/services.json");

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    store.LoadActionsFromJson(actionsJson);
    store.LoadServicesFromJson(servicesJson);

    var wjb = WJbBuilder.Create(store, cfg =>
    {
        cfg.AddActionsFromJson(actionsJson);
        cfg.AddServicesFromJson(servicesJson);
        cfg.AddService<IStore>(store);
    });

    return wjb;
});

builder.Services.AddSingleton<WasmWorker>();
builder.Services.AddSingleton<CronWorker>();

var app = builder.Build();

app.Services.GetRequiredService<WasmWorker>().Start();
app.Services.GetRequiredService<CronWorker>().Start();

await app.RunAsync();

//builder.Services.AddSingleton<IWJb>(sp =>
//{
//    var store = sp.GetRequiredService<IStore>();

//    var wjb = WJbBuilder.Create(store, cfg =>
//    {
//        // Basic demos
//        cfg.AddAction<HelloAction>("hello");
//        cfg.AddAction<ProgressAction>("progress");

//        // Configured
//        cfg.AddAction<ConfiguredAction>("configured");

//        // Chained
//        cfg.AddAction<SendEmailAction>("send-email");
//        cfg.AddAction<LogAction>("log");

//        // Retry Workflow
//        cfg.AddAction<RetryEmailAction>("retry-email");

//        // Http Ping
//        cfg.AddAction<HttpPingAction>(HttpPingAction.Key, new { cron = "* * * * *" });

//        // Order Workflow
//        cfg.AddAction<CreateOrderAction>("create-order");
//        cfg.AddAction<ReserveStockAction>("reserve-stock");
//        cfg.AddAction<ChargePaymentAction>("charge-payment");
//        cfg.AddAction<SendConfirmationAction>("send-confirmation");

//        cfg.AddService(new SmtpSettings
//        {
//            Host = "smtp.local"
//        });
//    });

//    return wjb;
//});
