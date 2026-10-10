using WJb;

var builder = WebApplication.CreateBuilder(args);

//  ...

builder.Services.AddSingleton<IStore, InMemoryStore>();

builder.Services.AddSingleton<IWJb>(sp =>
{
    var store = sp.GetRequiredService<IStore>();

    var wjb = WJbBuilder.Create(store, cfg =>
    {
        // Order Workflow
        cfg.AddAction<CreateOrderAction>(CreateOrderAction.Key);
        cfg.AddAction<ReserveStockAction>(ReserveStockAction.Key);
        cfg.AddAction<ChargePaymentAction>(ChargePaymentAction.Key);
        cfg.AddAction<SendConfirmationAction>(SendConfirmationAction.Key);
    });

    return wjb;
});

builder.Services.AddSingleton<WasmWorker>();

var app = builder.Build();

//  ...

app.Services.GetRequiredService<WasmWorker>().Start();

app.Run();