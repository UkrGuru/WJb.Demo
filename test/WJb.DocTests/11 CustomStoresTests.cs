using WJb.Helpers;

namespace WJb.DocTests;

public class _11_CustomStoresTests
{
    [Fact]
    public void Step_Should_Support_Object_Payload()
    {
        var step = Steps.Next(
            "send-email",
            new
            {
                Email = "user@test.com"
            });

        Assert.NotNull(step.Payload);
    }

    [Fact]
    public void Step_Should_Support_Integer_Payload_Value()
    {
        var step = Steps.Next(
            "customer",
            new
            {
                CustomerId = 42
            });

        var payload = step.Payload!.AsObject();

        Assert.Equal(
            42,
            payload["customerId"]!
                .GetValue<int>());
    }

    [Fact]
    public void Step_Should_Support_Array_Payload()
    {
        var step = Steps.Next(
            "numbers",
            new[] { 1, 2, 3 });

        var payload =
            JsonHelper.ToModel<int[]>(
                step.Payload);

        Assert.NotNull(payload);

        Assert.Equal(
            3,
            payload!.Length);
    }

    [Fact]
    public void Done_Should_Support_Integer_Value()
    {
        var result = Results.Done(123);

        Assert.Equal(
            123,
            result.Result!
                .GetValue<int>());
    }

    [Fact]
    public void Done_Should_Support_String_Value()
    {
        var result = Results.Done("Done");

        Assert.Equal(
            "Done",
            result.Result!
                .GetValue<string>());
    }

    [Fact]
    public void Done_Should_Support_Boolean_Value()
    {
        var result = Results.Done(true);

        Assert.True(
            result.Result!
                .GetValue<bool>());
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