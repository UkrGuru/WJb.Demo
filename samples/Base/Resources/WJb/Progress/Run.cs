@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        ProgressAction.Key,
        new ProgressPayload { DelayMs = 1000 });