using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MiscUtil
{
    /// <summary>
    /// This structure is used to remove the current SynchronizationContext. 
    /// This helps to reduce the verbosity of using <see cref="Task.ConfigureAwait"/>.
    /// <code>await new SynchronizationContextRemover();</code> 
    /// ...before awaiting anything else in the same method to remove the need 
    /// for ConfigureAwait.
    /// </summary>
    public readonly struct SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted => SynchronizationContext.Current is null;

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            var previous = SynchronizationContext.Current;

            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        public SynchronizationContextRemover GetAwaiter() => this;

        public void GetResult()
        {
        }
    }
}
