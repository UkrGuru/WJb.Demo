using WJb.Contracts;
using WJb.Core;

namespace WJb.Demo.Wasm;

public interface IJobClient
{
    Task<string> EnqueueAsync<TAction, TPayload>(TPayload payload);

    Task CancelAsync(string jobId);
}
public interface IJobQuery
{
    Task<IReadOnlyList<JobInfo>> GetJobs();
}

public interface IJobNotifier
{
    event Action? Changed;
}

public sealed class JobEngineLite :
    IJobClient,
    IJobQuery,
    IJobNotifier
{
    private readonly IStore _store;
    private readonly JobExecutor _executor;

    public event Action? Changed;

    public JobEngineLite(IStore store, JobExecutor executor)
    {
        _store = store;
        _executor = executor;

        _executor.Changed += Notify;
    }

    // ------------------------
    // Client
    // ------------------------

    public Task<string> EnqueueAsync<TAction, TPayload>(TPayload payload)
    {
        // ✅ НЕ обходить executor
        return _executor.EnqueueAsync<TAction>(payload);
    }

    public async Task CancelAsync(string jobId)
    {
        await _store.TrySetStateAsync(jobId, JobStatus.Failed);

        Notify(); // ✅ тут оставить
    }

    // ------------------------
    // Query
    // ------------------------

    public async Task<IReadOnlyList<JobInfo>> GetJobs()
        => await _store.GetJobsAsync(new JobQueryInfo());

    // ------------------------
    // Execution loop
    // ------------------------

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var executed = await _executor.ExecuteOnceAsync(ct);

            if (!executed)
                await Task.Delay(50, ct);
        }
    }

    private void Notify()
        => Changed?.Invoke();

    public void Dispose()
    {
        _executor.Changed -= Notify;
    }
}
