@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        HelloAction.Key,
        new HelloPayload { Text = "Hello World! ✅" });
