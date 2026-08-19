@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        ConfiguredAction.Key,
        new EmailInput { To = "test@test.com" });

