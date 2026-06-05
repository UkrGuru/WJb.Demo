using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WJb.Demo.Wasm;
using WJb.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddWJb()
    .UseMemory()
    .UseRetry()
    .ScanActions(typeof(DemoAction).Assembly);

var app = builder.Build();

await app.Services.UseWasmWorkerAsync();

await app.RunAsync();
