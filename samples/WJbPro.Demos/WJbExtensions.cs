using Microsoft.Extensions.DependencyInjection;
using WJb;

namespace WJbPro.Demos;

public static class WJbExtensions
{
    public static async Task AddWJbDemoAsync(this IServiceCollection services, int workers = 4, HttpClient? http = null)
    {
        string actionsJson;
        string servicesJson;

        if (http == null)
        {
            actionsJson = await File.ReadAllTextAsync("App_Data/actions.json");
            servicesJson = await File.ReadAllTextAsync("App_Data/services.json");
        }
        else
        {
            actionsJson = await http.GetStringAsync("App_Data/actions.json");
            servicesJson = await http.GetStringAsync("App_Data/services.json");
        }

        services.AddSingleton(new WJbStartupData
        {
            ActionsJson = actionsJson,
            ServicesJson = servicesJson
        });

        services.AddSingleton<IWJb>(sp =>
        {
            var store = sp.GetRequiredService<IStore>();

            return WJbBuilder.Create(store, cfg =>
            {
                cfg.AddActionsFromJson(actionsJson);
                cfg.AddServicesFromJson(servicesJson);
                cfg.AddService<IStore>(store);
            });
        });

        services.AddSingleton(sp =>
        {
            Func<IWJb> factory = () => sp.GetRequiredService<IWJb>();
            return new WasmWorkerPool(factory, count: workers);
        });

        services.AddSingleton<IWorkNotifier>(sp =>
            sp.GetRequiredService<WasmWorkerPool>());

        services.AddSingleton<CronWorker>();
    }

    public static async Task UseWJbDemoAsync(this IServiceProvider services, bool forceReloadDefinitions = true)
    {
        var store = services.GetRequiredService<IStore>();

        var data = services.GetRequiredService<WJbStartupData>();

        if (forceReloadDefinitions ||
            !(await store.GetListAsync(DefinitionType.Actions)).Any())
        {
            await store.LoadActionsFromJsonAsync(data.ActionsJson);
        }

        if (forceReloadDefinitions ||
            !(await store.GetListAsync(DefinitionType.Services)).Any())
        {
            await store.LoadServicesFromJsonAsync(data.ServicesJson);
        }

        services.GetRequiredService<WasmWorkerPool>().Start();
        services.GetRequiredService<CronWorker>().Start();
    }
}

internal sealed class WJbStartupData
{
    public string ActionsJson { get; init; } = string.Empty;

    public string ServicesJson { get; init; } = string.Empty;
}