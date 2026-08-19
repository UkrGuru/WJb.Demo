using WJb;

var builder = WebApplication.CreateBuilder(args);

//  ...

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    var wjb = WJbBuilder.Create(store, cfg =>
    {
        // Retry Workflow
        cfg.AddAction<RetryEmailAction>(RetryEmailAction.Key);
        cfg.AddAction<LogAction>(LogAction.Key);
    });

    return wjb;
});

builder.Services.AddSingleton<WasmWorker>();

var app = builder.Build();

//  ...

app.Services.GetRequiredService<WasmWorker>().Start();

app.Run();