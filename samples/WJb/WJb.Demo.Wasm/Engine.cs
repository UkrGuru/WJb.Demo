namespace WJb.Demo.Wasm;

public interface IJobClient
{
    Task<string> EnqueueAsync(
        string action,
        object? payload = null,
        JobOptions? options = null);

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
    private readonly IJobExecutor _executor;

    public event Action? Changed;

    public JobEngineLite(
        IStore store,
        IJobExecutor executor)
    {
        _store = store;
        _executor = executor;

        _executor.Changed += Notify;
    }

    // ------------------------
    // Client
    // ------------------------

    public Task<string> EnqueueAsync(
        string action,
        object? payload = null,
        JobOptions? options = null)
    {
        return _executor.EnqueueAsync(
            action,
            payload,
            options);
    }

    public Task CancelAsync(string jobId)
    {
        _executor.TryCancel(jobId);

        Notify();

        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string jobId)
    {
        await _store.DeleteJobAsync(jobId);

        Notify();
    }

    public async Task CleanAsync()
    {
        var jobs = await _store.GetJobsAsync(
            new JobQuery());

        var ids = jobs
            .Where(x => x.Status is JobStatus.Completed or JobStatus.Failed)
            .Select(x => x.Id)
            .ToList();

        foreach (var id in ids)
        {
            await _store.DeleteJobAsync(id);
        }

        Notify();
    }

    // ------------------------
    // Query
    // ------------------------

    public Task<IReadOnlyList<JobInfo>> GetJobs()
        => _store.GetJobsAsync(new JobQuery());

    // ------------------------
    // Execution loop
    // ------------------------

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var executed =
                await _executor.ExecuteOnceAsync(ct);

            if (!executed)
            {
                await Task.Delay(50, ct);
            }
        }
    }

    // ------------------------
    // Notification
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