namespace MiscUtil;

public static partial class TaskEx
{
    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
    {
        static async ValueTask<(T1, T2)> Awaited(Task<T1> t1, Task<T2> t2)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2);
        }

#if NETSTANDARD2_0
        if (task1.Status == TaskStatus.RanToCompletion &&
            task2.Status == TaskStatus.RanToCompletion)
#else
        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully)
#endif
        {
            return new ValueTask<(T1, T2)>((task1.Result, task2.Result));
        }

        return Awaited(task1, task2);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2)> WhenAll<T1, T2>(ValueTask<T1> task1, ValueTask<T2> task2)
    {
        static async ValueTask<(T1, T2)> Awaited(ValueTask<T1> t1, ValueTask<T2> t2)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2);
        }

        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully)
        {
            return new ValueTask<(T1, T2)>((task1.Result, task2.Result));
        }

        return Awaited(task1, task2);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
    {
        static async ValueTask<(T1, T2, T3)> Awaited(Task<T1> t1, Task<T2> t2, Task<T3> t3)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3);
        }

#if NETSTANDARD2_0
        if (task1.Status == TaskStatus.RanToCompletion &&
            task2.Status == TaskStatus.RanToCompletion &&
            task3.Status == TaskStatus.RanToCompletion)
#else
        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully)
#endif
        {
            return new ValueTask<(T1, T2, T3)>((task1.Result, task2.Result, task3.Result));
        }

        return Awaited(task1, task2, task3);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3)> WhenAll<T1, T2, T3>(ValueTask<T1> task1, ValueTask<T2> task2, ValueTask<T3> task3)
    {
        static async ValueTask<(T1, T2, T3)> Awaited(ValueTask<T1> t1, ValueTask<T2> t2, ValueTask<T3> t3)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3);
        }

        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully)
        {
            return new ValueTask<(T1, T2, T3)>((task1.Result, task2.Result, task3.Result));
        }

        return Awaited(task1, task2, task3);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
    {
        static async ValueTask<(T1, T2, T3, T4)> Awaited(Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3, await t4);
        }

#if NETSTANDARD2_0
        if (task1.Status == TaskStatus.RanToCompletion &&
            task2.Status == TaskStatus.RanToCompletion &&
            task3.Status == TaskStatus.RanToCompletion &&
            task4.Status == TaskStatus.RanToCompletion)
#else
        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully && task4.IsCompletedSuccessfully)
#endif
        {
            return new ValueTask<(T1, T2, T3, T4)>((task1.Result, task2.Result, task3.Result, task4.Result));
        }

        return Awaited(task1, task2, task3, task4);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(ValueTask<T1> task1, ValueTask<T2> task2, ValueTask<T3> task3, ValueTask<T4> task4)
    {
        static async ValueTask<(T1, T2, T3, T4)> Awaited(ValueTask<T1> t1, ValueTask<T2> t2, ValueTask<T3> t3, ValueTask<T4> t4)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3, await t4);
        }

        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully && task4.IsCompletedSuccessfully)
        {
            return new ValueTask<(T1, T2, T3, T4)>((task1.Result, task2.Result, task3.Result, task4.Result));
        }

        return Awaited(task1, task2, task3, task4);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
    {
        static async ValueTask<(T1, T2, T3, T4, T5)> Awaited(Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4, Task<T5> t5)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3, await t4, await t5);
        }

#if NETSTANDARD2_0
        if (task1.Status == TaskStatus.RanToCompletion &&
            task2.Status == TaskStatus.RanToCompletion &&
            task3.Status == TaskStatus.RanToCompletion &&
            task4.Status == TaskStatus.RanToCompletion &&
            task5.Status == TaskStatus.RanToCompletion)
#else
        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully && task4.IsCompletedSuccessfully &&
            task5.IsCompletedSuccessfully)
#endif
        {
            return new ValueTask<(T1, T2, T3, T4, T5)>((task1.Result, task2.Result, task3.Result, task4.Result, task5.Result));
        }

        return Awaited(task1, task2, task3, task4, task5);
    }

    /// <summary>
    /// Creates a task that will complete when all of the supplied tasks have completed. Does not require
    /// all tasks to return the same type.
    /// </summary>
    public static ValueTask<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(ValueTask<T1> task1, ValueTask<T2> task2, ValueTask<T3> task3, ValueTask<T4> task4, ValueTask<T5> task5)
    {
        static async ValueTask<(T1, T2, T3, T4, T5)> Awaited(ValueTask<T1> t1, ValueTask<T2> t2, ValueTask<T3> t3, ValueTask<T4> t4, ValueTask<T5> t5)
        {
            await new SynchronizationContextRemover();
            return (await t1, await t2, await t3, await t4, await t5);
        }

        if (task1.IsCompletedSuccessfully && task2.IsCompletedSuccessfully &&
            task3.IsCompletedSuccessfully && task4.IsCompletedSuccessfully &&
            task5.IsCompletedSuccessfully)
        {
            return new ValueTask<(T1, T2, T3, T4, T5)>((task1.Result, task2.Result, task3.Result, task4.Result, task5.Result));
        }

        return Awaited(task1, task2, task3, task4, task5);
    }
}
