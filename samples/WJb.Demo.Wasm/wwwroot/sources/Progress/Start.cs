await Store.EnqueueAsync(
    ProgressAction.Key,
    new ProgressPayload
    {
        DelayMs = 300
    });