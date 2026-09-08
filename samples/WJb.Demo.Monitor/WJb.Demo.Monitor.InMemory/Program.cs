using WJb;
using WJb.Demo.Monitor.InMemory.Components;
using WJbPro.Demos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// WJb store implementation.
builder.Services.AddSingleton<IStore, InMemoryStore>();

// WJb demo setup: load JSON definitions and register WJb services.
await builder.Services.AddWJbDemoAsync();

// or use SqlStore for testing purposes
// using Microsoft.Data.SqlClient;
// using WJb.Sql;
// const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=WJbMonitor;Trusted_Connection=True;TrustServerCertificate=True;";
// await using (var conn = new SqlConnection(connectionString))
// { await conn.InitDbAsync(); }
// var store = new SqlStore(() => new SqlConnection(connectionString));

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

// WJb demo startup: initialize store and start background workers.
await app.Services.UseWJbDemoAsync(forceReloadDefinitions: true);

app.Run();
