using WJb.Contracts;
using WJb.Core;

namespace WJb.Demo.Wasm;

public interface IJobClient
{
    Task<string> EnqueueAsync<TAction, TPayload>(TPayload payload);
    Task CancelAsync(string jobId);

    Task DeleteAsync(string jobId);
    Task CleanAsync();
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
    IJobNotifier,
    IDisposable
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
        return _executor.EnqueueAsync<TAction>(payload);
    }

    public Task CancelAsync(string jobId)
    {
        _executor.TryCancel(jobId);

        Notify();

        return Task.CompletedTask;
    }

    // ✅ Delete single job
    public async Task DeleteAsync(string jobId)
    {
        await _store.DeleteJobAsync(jobId);

        Notify();
    }

    // ✅ Clean completed/failed jobs
    public async Task CleanAsync()
    {
        var jobs = await _store.GetJobsAsync(new JobQueryInfo());

        var toDelete = jobs
            .Where(x => x.Status is JobStatus.Completed or JobStatus.Failed)
            .Select(x => x.Id)
            .ToList();

        foreach (var id in toDelete)
        {
            await _store.DeleteJobAsync(id);
        }

        Notify();
    }

    // ------------------------
    // Query
    // ------------------------

    public Task<IReadOnlyList<JobInfo>> GetJobs()
        => _store.GetJobsAsync(new JobQueryInfo());

    // ------------------------
    // Execution loop
    // ------------------------

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var executed = await _executor.ExecuteOnceAsync(ct);

            if (!executed)
            {
                await Task.Delay(50, ct);
            }
        }
    }

    // ------------------------
    // Notify
    // ------------------------

    private void Notify()
        => Changed?.Invoke();

    // ------------------------
    // Dispose
    // ------------------------

    public void Dispose()
    {
        _executor.Changed -= Notify;
    }
}