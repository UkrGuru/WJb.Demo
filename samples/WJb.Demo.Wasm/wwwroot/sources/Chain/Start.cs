await Store.EnqueueAsync(
    SendEmailAction.Key,
    new EmailInput
    {
        To = "test@test.com"
    });