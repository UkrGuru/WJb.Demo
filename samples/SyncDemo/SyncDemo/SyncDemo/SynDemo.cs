using System.Text.Json;
using WJb;

namespace SyncDemo;

public sealed class Customer
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";
}

public enum SyncPhase
{
    Delete,
    Update,
    Create,
    Completed
}

public sealed class CustomerSyncState
{
    public SyncPhase Phase { get; set; }

    public int Index { get; set; }
}

public sealed class SyncPlan
{
    public List<Customer> Deletes { get; } = [];

    public List<Customer> Updates { get; } = [];

    public List<Customer> Creates { get; } = [];
}

public sealed class JsonFileStore
{
    public async Task<List<T>> LoadAsync<T>(string file)
    {
        if (!File.Exists(file))
            return [];

        await using var stream =
            File.OpenRead(file);

        return await JsonSerializer.DeserializeAsync<List<T>>(stream)
               ?? [];
    }

    public async Task<T?> LoadObjectAsync<T>(string file)
    {
        if (!File.Exists(file))
            return default;

        await using var stream =
            File.OpenRead(file);

        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    public async Task SaveObjectAsync<T>(
        string file,
        T value)
    {
        Directory.CreateDirectory(
            Path.GetDirectoryName(file)!);

        await using var stream =
            File.Create(file);

        await JsonSerializer.SerializeAsync(
            stream,
            value,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
    }
}

public sealed class CustomerSyncService
{
    private readonly JsonFileStore _store = new();

    public async Task<SyncPlan> BuildPlanAsync()
    {
        var source =
            await _store.LoadAsync<Customer>(
                "App_Data/Source/customers.json");

        var target =
            await _store.LoadAsync<Customer>(
                "App_Data/Target/customers.json");

        var plan = new SyncPlan();

        var sourceIds =
            source.Select(x => x.Id)
                  .ToHashSet();

        foreach (var targetCustomer in target)
        {
            if (!sourceIds.Contains(targetCustomer.Id))
            {
                plan.Deletes.Add(targetCustomer);
            }
        }

        foreach (var sourceCustomer in source)
        {
            var targetCustomer =
                target.FirstOrDefault(
                    x => x.Id == sourceCustomer.Id);

            if (targetCustomer is null)
            {
                plan.Creates.Add(sourceCustomer);
                continue;
            }

            if (targetCustomer.Name != sourceCustomer.Name ||
                targetCustomer.Email != sourceCustomer.Email)
            {
                plan.Updates.Add(sourceCustomer);
            }
        }

        return plan;
    }
}

public sealed record CustomerSyncPayload;


[ActionName("customer-sync")]
public sealed class CustomerSyncAction
    : JobAction<CustomerSyncPayload?>
{
    public static TaskCompletionSource Completion { get; set; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private const string CheckpointFile = "App_Data/State/checkpoint.json";

    private readonly CustomerSyncService _sync =
        new();

    private readonly JsonFileStore _store =
        new();

    public override async ValueTask<ActionResult> ExecuteAsync(
        CustomerSyncPayload? payload,
        CancellationToken ct = default)
    {
        var state =
            await LoadStateAsync();

        var plan =
            await _sync.BuildPlanAsync();

        while (state.Phase != SyncPhase.Completed &&
               !ct.IsCancellationRequested)
        {
            switch (state.Phase)
            {
                case SyncPhase.Delete:
                    await ExecuteDeleteAsync(
                        state,
                        plan,
                        ct);
                    break;

                case SyncPhase.Update:
                    await ExecuteUpdateAsync(
                        state,
                        plan,
                        ct);
                    break;

                case SyncPhase.Create:
                    await ExecuteCreateAsync(
                        state,
                        plan,
                        ct);
                    break;
            }
        }

        Console.WriteLine();
        Console.WriteLine("SYNC COMPLETED");
        Console.WriteLine();

        Completion.TrySetResult();

        return ActionResult.Empty;
    }

    private async Task ExecuteDeleteAsync(
        CustomerSyncState state,
        SyncPlan plan,
        CancellationToken ct)
    {
        while (state.Index < plan.Deletes.Count)
        {
            var customer =
                plan.Deletes[state.Index];

            Console.WriteLine(
                $"DELETE {customer.Id}");

            state.Index++;

            await SaveStateAsync(state);

            await Task.Delay(1000, ct);
        }

        Console.WriteLine();
        Console.WriteLine("DELETE phase completed");
        Console.WriteLine();

        state.Phase = SyncPhase.Update;
        state.Index = 0;

        await SaveStateAsync(state);
    }

    private async Task ExecuteUpdateAsync(
        CustomerSyncState state,
        SyncPlan plan,
        CancellationToken ct)
    {
        while (state.Index < plan.Updates.Count)
        {
            var customer =
                plan.Updates[state.Index];

            Console.WriteLine(
                $"UPDATE {customer.Id}");

            state.Index++;

            await SaveStateAsync(state);

            await Task.Delay(1000, ct);
        }

        Console.WriteLine();
        Console.WriteLine("UPDATE phase completed");
        Console.WriteLine();

        state.Phase = SyncPhase.Create;
        state.Index = 0;

        await SaveStateAsync(state);
    }

    private async Task ExecuteCreateAsync(
        CustomerSyncState state,
        SyncPlan plan,
        CancellationToken ct)
    {
        while (state.Index < plan.Creates.Count)
        {
            var customer =
                plan.Creates[state.Index];

            Console.WriteLine(
                $"CREATE {customer.Id}");

            state.Index++;

            await SaveStateAsync(state);

            await Task.Delay(1000, ct);
        }

        Console.WriteLine();
        Console.WriteLine("CREATE phase completed");
        Console.WriteLine();

        state.Phase = SyncPhase.Completed;

        await SaveStateAsync(state);
    }

    private async Task<CustomerSyncState> LoadStateAsync()
    {
        return await _store.LoadObjectAsync<CustomerSyncState>(
                   CheckpointFile)
               ?? new CustomerSyncState
               {
                   Phase = SyncPhase.Delete,
                   Index = 0
               };
    }

    private Task SaveStateAsync(
        CustomerSyncState state)
    {
        return _store.SaveObjectAsync(
            CheckpointFile,
            state);
    }
}

public static class Runner
{
    public static async Task ExecuteAsync()
    {
        CustomerSyncAction.Completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        var store = new InMemoryStore();

        var wjb =
            WJbBuilder.Create(store, cfg =>
            {
                cfg.AddAction<CustomerSyncAction>(
                    "customer-sync");
            });

        using var cts =
            new CancellationTokenSource();

        var worker = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await wjb.ExecuteLoopAsync();

                await Task.Delay(
                    100,
                    cts.Token);
            }
        });

        await wjb.EnqueueAsync(
            "customer-sync",
            new CustomerSyncPayload());

        Console.WriteLine(
            "Sync job queued.");

        Console.WriteLine();

        await CustomerSyncAction.Completion.Task;

        cts.Cancel();

        try
        {
            await worker;
        }
        catch (OperationCanceledException)
        {
        }
    }
}