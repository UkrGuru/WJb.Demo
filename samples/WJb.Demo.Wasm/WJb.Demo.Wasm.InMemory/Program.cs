using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb;
using WJb.Demo.Wasm.InMemory;
using WJbPro.Demos;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var http = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

builder.Services.AddSingleton(http);

// WJb store implementation.
builder.Services.AddSingleton<IStore, InMemoryStore>();

// WJb demo setup: load JSON definitions and register WJb services.
await builder.Services.AddWJbDemoAsync(http);

var app = builder.Build();

// WJb demo startup: initialize store and start background workers.
await app.Services.UseWJbDemoAsync(forceReloadDefinitions: true);

await app.RunAsync();