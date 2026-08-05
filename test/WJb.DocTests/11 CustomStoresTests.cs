namespace WJb.DocTests;

public class _11_CustomStoresTests
{
    [Fact]
    public void JobCommand_Should_Support_Object_Payload()
    {
        var command = new JobCommand(
            "send-email",
            new
            {
                Email = "user@test.com"
            });

        Assert.NotNull(command.Payload);
    }

    [Fact]
    public void JobCommand_Should_Support_Integer_Payload_Value()
    {
        var command = new JobCommand(
            "customer",
            new
            {
                CustomerId = 42
            });

        var payload = command.AsObject();

        Assert.Equal(
            42,
            payload!["CustomerId"]!.GetValue<int>());
    }

    [Fact]
    public void JobCommand_Should_Support_Array_Payload()
    {
        var command = new JobCommand(
            "numbers",
            new[] { 1, 2, 3 });

        var payload = command.GetPayload<int[]>();

        Assert.NotNull(payload);
        Assert.Equal(3, payload.Length);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Integer_Value()
    {
        var result = ActionResults.Result(123);

        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_String_Value()
    {
        var result = ActionResults.Result("Done");

        Assert.Equal("Done", result.Value);
    }

    [Fact]
    public void ActionResults_Result_Should_Support_Boolean_Value()
    {
        var result = ActionResults.Result(true);

        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void JobOptions_Should_Support_Queue()
    {
        var options = new JobOptions
        {
            Queue = "email"
        };

        Assert.Equal(
            "email",
            options.Queue);
    }

    [Fact]
    public void JobOptions_Should_Support_Delay()
    {
        var options = new JobOptions
        {
            Delay = TimeSpan.FromMinutes(5)
        };

        Assert.Equal(
            TimeSpan.FromMinutes(5),
            options.Delay);
    }

    [Fact]
    public void InMemoryStore_Should_Be_Usable_As_Reference_Implementation()
    {
        var store = new InMemoryStore();

        Assert.NotNull(store);
    }
}