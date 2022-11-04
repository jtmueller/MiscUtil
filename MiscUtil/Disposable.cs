namespace MiscUtil;

public static class Disposable
{
    public static DisposeScope Create(Action onDispose) => new(onDispose);

    public ref struct DisposeScope
    {
        private Action? _onDispose;

        public DisposeScope(Action onDispose) => _onDispose = onDispose;

        public void Dispose() => Interlocked.Exchange(ref _onDispose, null)?.Invoke();
    }
}

#if !NETSTANDARD2_0

public static class AsyncDisposable
{
    public static AsyncDisposeScope Create(Func<ValueTask> onDispose) => new(onDispose);

    public struct AsyncDisposeScope
    {
        private Func<ValueTask>? _onDispose;

        public AsyncDisposeScope(Func<ValueTask> onDispose) => _onDispose = onDispose;

        public ValueTask DisposeAsync() => Interlocked.Exchange(ref _onDispose, null)?.Invoke() ?? ValueTask.CompletedTask;
    }
}

#endif