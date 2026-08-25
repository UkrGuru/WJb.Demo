@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        CleanUpAction.Key, null);
