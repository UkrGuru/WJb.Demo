using WJb;
using WJb.Demo.Monitor.InMemory.Components;
using WJbPro.Demos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// WJb store implementation.
builder.Services.AddSingleton<IStore, InMemoryStore>();

// WJb demo setup.
await builder.Services.AddWJbDemoAsync(workers: 8);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// WJb demo startup.
await app.Services.UseWJbDemoAsync(
    forceReloadDefinitions: true);

app.Run();