// using Microsoft.Data.SqlClient;
using WJb;
using WJb.Demo.Monitor;

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

    store.LoadActionsFromJson(
        File.ReadAllText("App_Data/actions.json"));


    var actions =
        store.GetListAsync(DefinitionType.Actions)
             .GetAwaiter()
             .GetResult();

    Console.WriteLine("ACTIONS:");

    foreach (var action in actions)
    {
        Console.WriteLine($"{action.Key}");
        Console.WriteLine(action.Value);
    }


    store.LoadServicesFromJson(
        File.ReadAllText("App_Data/services.json"));

    return store;
});


builder.Services.AddSingleton<IWJbExecutor>(sp =>
{
    Console.WriteLine("IWJbExecutor created");

    var store = sp.GetRequiredService<IStore>();

    return WJbBuilder.CreateAsync(store)
        .GetAwaiter()
        .GetResult();
});


builder.Services.AddHostedService<WJbWorker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//var wjb = app.Services.GetRequiredService<IWJbExecutor>();

//_ = Task.Run(async () =>
//{
//    await wjb.EnqueueAsync(
//        ImportCustomersAction.Key,
//        new ImportCustomersInput());

//    await wjb.ExecuteLoopAsync();
//});

app.Run();