using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb;
using WJb.Demo.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<HttpClient>(_ =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

var http = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

var actionsJson = await http.GetStringAsync("App_Data/actions.json");
var servicesJson = await http.GetStringAsync("App_Data/services.json");

builder.Services.AddSingleton<IStore, InMemoryStore>();
//builder.Services.AddSingleton<IStore, IdbStore>();

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
