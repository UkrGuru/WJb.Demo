using WJb;
using WJb.Demo.Monitor;
using WJb.Demo.Monitor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// use InMemoryStore for testing purposes
var store = new InMemoryStore();

// or use SqlStore for testing purposes
// using Microsoft.Data.SqlClient;
// using WJb.Sql;
// const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=WJbMonitor;Trusted_Connection=True;TrustServerCertificate=True;";
// await using (var conn = new SqlConnection(connectionString))
// { await conn.InitDbAsync(); }
// var store = new SqlStore(() => new SqlConnection(connectionString));

await store.AddActionAsync<ImportCustomersAction>(Actions.ImportCustomers);
await store.AddActionAsync<GenerateReportAction>(Actions.GenerateReport);
await store.AddActionAsync<SendEmailAction>(Actions.SendEmail);

await store.AddServiceAsync(new SmtpSettings
{
    Host = "smtp.demo.local",
    Port = 25,
    From = "noreply@demo.local"
});

builder.Services.AddSingleton<IStore>(store);

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();
    return WJbBuilder.CreateAsync(store).GetAwaiter().GetResult();
});

builder.Services.AddSingleton<WasmWorker>();
// builder.Services.AddSingleton<CronWorker>();

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

app.Services.GetRequiredService<WasmWorker>().Start();
// app.Services.GetRequiredService<CronWorker>().Start();

app.Run();

