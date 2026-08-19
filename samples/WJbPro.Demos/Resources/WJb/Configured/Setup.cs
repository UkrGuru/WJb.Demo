using WJb;

var builder = WebApplication.CreateBuilder(args);

//  ...

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    var wjb = WJbBuilder.Create(store, cfg =>
    {
        // Configured
        cfg.AddAction<ConfiguredAction>(ConfiguredAction.Key);
        cfg.AddService(new SmtpSettings
        {
            Host = "smtp.local"
        });
    });

    return wjb;
});

builder.Services.AddSingleton<WasmWorker>();

var app = builder.Build();

//  ...

app.Services.GetRequiredService<WasmWorker>().Start();

app.Run();
