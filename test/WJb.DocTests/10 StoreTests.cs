namespace WJb.DocTests;

public class _10_StoreTests
{
    [Fact]
    public void InMemoryStore_Should_Be_Creatable()
    {
        var store = new InMemoryStore();

        Assert.NotNull(store);
    }

    [Fact]
    public void Step_Should_Support_Guid_Payload_Values()
    {
        var id = Guid.NewGuid();

        var step = Steps.Next(
            "test",
            new
            {
                Id = id
            });

        Assert.NotNull(step.Payload);
    }

    [Fact]
    public void Step_Should_Support_Integer_Payload_Values()
    {
        var step = Steps.Next(
            "test",
            new
            {
                CustomerId = 42
            });

        var payload = step.Payload!.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            42,
            payload["customerId"]!
                .GetValue<int>());
    }

    [Fact]
    public void Step_Should_Support_String_Payload_Values()
    {
        var step = Steps.Next(
            "test",
            new
            {
                File = "report.pdf"
            });

        var payload = step.Payload!.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "report.pdf",
            payload["file"]!
                .GetValue<string>());
    }

    [Fact]
    public void Done_Should_Support_Integer_Result()
    {
        var result = Results.Done(123);

        Assert.Equal(
            123,
            result.Result!
                .GetValue<int>());
    }

    [Fact]
    public void Done_Should_Support_String_Result()
    {
        var result = Results.Done("Done");

        Assert.Equal(
            "Done",
            result.Result!
                .GetValue<string>());
    }

    [Fact]
    public void Done_Should_Support_Object_Result()
    {
        var result = Results.Done(
            new
            {
                Sent = true
            });

        Assert.NotNull(
            result.Result);
    }

    [Fact]
    public void Step_Should_Support_Small_Metadata_Payloads()
    {
        var step = Steps.Next(
            "send-email",
            new
            {
                BodyId = "html-123"
            });

        var payload = step.Payload!.AsObject();

        Assert.NotNull(payload);

        Assert.Equal(
            "html-123",
            payload["bodyId"]!
                .GetValue<string>());
    }
}