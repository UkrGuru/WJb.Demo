// using Microsoft.Data.SqlClient;
using WJb;

// using WJb.Sql;
using WJb.Demo.Monitor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddSingleton<IStore>(_ =>
//    new SqlStore(() => new SqlConnection(connectionString)));

builder.Services.AddSingleton<IStore>(_ =>
{
    var store = new InMemoryStore();

    store.LoadActionsFromJson(        File.ReadAllText("App_Data/actions.json"));

    store.LoadServicesFromJson(        File.ReadAllText("App_Data/services.json"));

    return store;
});

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    return WJbBuilder.CreateAsync(store)
        .GetAwaiter().GetResult();
});


builder.Services.AddSingleton<WasmWorker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Services.GetRequiredService<WasmWorker>().Start();

app.Run();