using BenchmarkDotNet.Attributes;

namespace MiscUtil.Benchmarks;

[Config(typeof(BenchmarkConfig))]
public class TaskEx_WhenAll
{
    [Benchmark]
    public async ValueTask TaskDelayed()
    {
        var (_, _) = await TaskEx.WhenAll(TaskIntDelayed(), TaskStringDelayed());
    }

    [Benchmark]
    public async ValueTask TaskCompleted()
    {
        var (_, _) = await TaskEx.WhenAll(TaskIntCompleted, TaskStringCompleted);
    }

    [Benchmark]
    public async ValueTask ValueTaskDelayed()
    {
        var (_, _) = await TaskEx.WhenAll(ValueTaskIntDelayed(), ValueTaskStringDelayed());
    }

    [Benchmark]
    public async ValueTask ValueTaskCompleted()
    {
        var (_, _) = await TaskEx.WhenAll(ValueTaskIntCompleted, ValueTaskStringCompleted);
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
