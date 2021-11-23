using Xunit;

namespace MiscUtil.Tests;

public class TaskExTests
{
    [Fact]
    public async ValueTask TasksDelayed()
    {
        var (intVal, stringVal) = await TaskEx.WhenAll(TaskIntDelayed(), TaskStringDelayed());
        Assert.Equal(42, intVal);
        Assert.Equal("42", stringVal);
    }

    [Fact]
    public async ValueTask ValueTasksDelayed()
    {
        var (intVal, stringVal) = await TaskEx.WhenAll(ValueTaskIntDelayed(), ValueTaskStringDelayed());
        Assert.Equal(42, intVal);
        Assert.Equal("42", stringVal);
    }

    [Fact]
    public async ValueTask TasksCompleted()
    {
        var (intVal, stringVal) = await TaskEx.WhenAll(TaskIntCompleted, TaskStringCompleted);
        Assert.Equal(42, intVal);
        Assert.Equal("42", stringVal);
    }

    [Fact]
    public async ValueTask ValueTasksCompleted()
    {
        var (intVal, stringVal) = await TaskEx.WhenAll(ValueTaskIntCompleted, ValueTaskStringCompleted);
        Assert.Equal(42, intVal);
        Assert.Equal("42", stringVal);
    }

    private async Task<int> TaskIntDelayed()
    {
        await Task.Delay(1);
        return 42;
    }

    private async Task<string> TaskStringDelayed()
    {
        await Task.Delay(1);
        return "42";
    }

    private readonly Task<int> TaskIntCompleted = Task.FromResult(42);

    private readonly Task<string> TaskStringCompleted = Task.FromResult("42");

    private async ValueTask<int> ValueTaskIntDelayed()
    {
        await Task.Delay(1);
        return 42;
    }

    private async ValueTask<string> ValueTaskStringDelayed()
    {
        await Task.Delay(1);
        return "42";
    }

    private readonly ValueTask<int> ValueTaskIntCompleted = new ValueTask<int>(42);

    private readonly ValueTask<string> ValueTaskStringCompleted = new ValueTask<string>("42");
}
