namespace MiscUtil;

public static class Disposable
{
    public static IDisposable Create(Action onDispose) => new DisposeScope(onDispose);

    private sealed class DisposeScope : IDisposable
    {
        private Action? _onDispose;

        public DisposeScope(Action onDispose) { _onDispose = onDispose; }

        public void Dispose() => Interlocked.Exchange(ref _onDispose, null)?.Invoke();
    }
}

#if !NETSTANDARD2_0

public static class AsyncDisposable
{
    public static IAsyncDisposable Create(Func<ValueTask> onDispose) => new AsyncDisposeScope(onDispose);

    private sealed class AsyncDisposeScope : IAsyncDisposable
    {
        private Func<ValueTask>? _onDispose;

        public AsyncDisposeScope(Func<ValueTask> onDispose) { _onDispose = onDispose; }

        public ValueTask DisposeAsync() => Interlocked.Exchange(ref _onDispose, null)?.Invoke() ?? ValueTask.CompletedTask;
    }
}

#endif