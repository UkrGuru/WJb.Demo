await Store.EnqueueAsync(
    CreateOrderAction.Key,
    new OrderInput
    {
        OrderId = 1001
    });