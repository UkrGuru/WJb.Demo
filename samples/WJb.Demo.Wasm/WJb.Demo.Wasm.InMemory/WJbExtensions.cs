using Microsoft.Extensions.DependencyInjection;
using WJb;

namespace WJb.Demo.Wasm.InMemory;

public static class WJbExtensions
{
    public static void AddWJbInMemory(
        this IServiceCollection services)
    {
        services.AddSingleton<WasmWorkerPool>(sp =>
        {
            Func<IWJb> factory =
                () => sp.GetRequiredService<IWJb>();

            return new WasmWorkerPool(
                factory,
                count: 4);
        });

        services.AddSingleton<IWorkNotifier>(sp =>
            sp.GetRequiredService<WasmWorkerPool>());

        services.AddSingleton<IStore>(sp =>
        {
            var notifier =
                sp.GetRequiredService<IWorkNotifier>();

            return new InMemoryStore(notifier);
        });

        services.AddSingleton<CronWorker>();
    }

    public static void UseWJbInMemory(
        this IServiceProvider services)
    {
        services
            .GetRequiredService<WasmWorkerPool>()
            .Start();

        services
            .GetRequiredService<CronWorker>()
            .Start();
    }
}