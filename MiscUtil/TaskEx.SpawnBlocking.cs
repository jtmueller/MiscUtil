#if NET6_0_OR_GREATER

using Microsoft.Extensions.ObjectPool;

namespace MiscUtil;

// Inspired by Tokio's spawn_blocking
// https://docs.rs/tokio/latest/tokio/task/fn.spawn_blocking.html

public static partial class TaskEx
{
    /// <summary>
    /// Runs the provided <see cref="Action"/> on a thread where blocking is acceptable.
    /// <para>
    ///   In general, issuing a blocking call or performing a lot of compute in an async method without yielding
    ///   is problematic, as it may prevent the async runtime from driving other async methods forward. This function
    ///   runs the provided action on a thread dedicated to blocking operations.
    /// </para>
    /// </summary>
    public static Task SpawnBlocking(Action action)
        => BlockingThreadPool.Run(action);

    /// <summary>
    /// Runs the provided <see cref="Func{TResult}"/> on a thread where blocking is acceptable.
    /// <para>
    ///   In general, issuing a blocking call or performing a lot of compute in an async method without yielding
    ///   is problematic, as it may prevent the async runtime from driving other async methods forward. This function
    ///   runs the provided function on a thread dedicated to blocking operations.
    /// </para>
    /// </summary>
    public static Task<T> SpawnBlocking<T>(Func<T> action)
        => BlockingThreadPool<T>.Run(action);
}

internal static class BlockingThreadPool
{
    // TODO: make maxRetained configurable. Environment variable?
    private static readonly DefaultObjectPool<Thread> _pool =
        new(new BlockingThreadPoolPolicy(ThreadStart), Environment.ProcessorCount * 2);

    private static void ThreadStart(object? state)
    {
        if (state is (Action action, TaskCompletionSource tcs))
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            finally
            {
                _pool.Return(Thread.CurrentThread);
            }
        }
    }

    public static Task Run(Action action)
    {
        var tcs = new TaskCompletionSource();
        var thread = _pool.Get();
        thread.Start((action, tcs));
        return tcs.Task;
    }
}

internal static class BlockingThreadPool<T>
{
    // TODO: make maxRetained configurable. Environment variable?
    private static readonly DefaultObjectPool<Thread> _pool =
        new(new BlockingThreadPoolPolicy(ThreadStart), Environment.ProcessorCount * 2);

    private static void ThreadStart(object? state)
    {
        if (state is (Func<T> action, TaskCompletionSource<T> tcs))
        {
            try
            {
                var result = action();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            finally
            {
                _pool.Return(Thread.CurrentThread);
            }
        }
    }

    public static Task<T> Run(Func<T> action)
    {
        var tcs = new TaskCompletionSource<T>();
        var thread = _pool.Get();
        thread.Start((action, tcs));
        return tcs.Task;
    }
}

internal sealed class BlockingThreadPoolPolicy : IPooledObjectPolicy<Thread>
{
    private static readonly TimeSpan JoinTimeout = TimeSpan.FromMilliseconds(10);

    private readonly ParameterizedThreadStart _action;

    public BlockingThreadPoolPolicy(ParameterizedThreadStart action)
    {
        _action = action;
    }

    public Thread Create() => new Thread(_action) { IsBackground = true };

    public bool Return(Thread thread)
    {
        return thread.Join(JoinTimeout);
    }
}

#endif