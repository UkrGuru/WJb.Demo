@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        HttpPingAction.Key,
        new HttpPingPayload { Url = "https://wjb.pro/" });
