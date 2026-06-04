using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb.Demo.Wasm;
using WJb.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services
    .AddWJb()
    .UseMemory()
    .UseRetry()
    .AddActions(b =>
    {
        b.Scan(typeof(DemoAction).Assembly);
    });

var app = builder.Build();

await app.Services.UseWasmWorkerAsync();

await app.RunAsync();
