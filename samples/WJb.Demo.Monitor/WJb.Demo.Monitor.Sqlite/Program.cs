using WJb;
using WJb.Sqlite;
using WJbPro.Demos;
using WJb.Demo.Monitor.Sqlite.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Read the SQL Server connection string from configuration.
var connectionString = builder.Configuration.GetConnectionString("Sqlite")
    ?? throw new InvalidOperationException("ConnectionStrings:Sqlite is missing.");

// Create the WJb database schema if it does not already exist.
await WJbSqlite.InitDbAsync(connectionString);

// Register the SQL Server-backed WJb store.
// A new SqlConnection is created for each store operation.
builder.Services.AddSingleton<IStore>(_ => new SqliteStore(() => new (connectionString)));

// WJb demo setup: load JSON definitions and register WJb services.
await builder.Services.AddWJbDemoAsync(workers: 4);

// WJb demo setup: load JSON definitions and register WJb services.
await builder.Services.AddWJbDemoAsync();

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
