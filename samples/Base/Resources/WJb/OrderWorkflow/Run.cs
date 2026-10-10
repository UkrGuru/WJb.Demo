@inject IStore Store

private async Task Run()
    => await Store.EnqueueAsync(
        CreateOrderAction.Key,
        new OrderInput { OrderId = 1001 });
