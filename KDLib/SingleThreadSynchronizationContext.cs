using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public class SingleThreadSynchronizationContext : SynchronizationContext
  {
    private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue = new();

    public override void Post(SendOrPostCallback d, object state)
    {
      _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
    }

    public void RunOnCurrentThread()
    {
      KeyValuePair<SendOrPostCallback, object> workItem;
      while (_queue.TryTake(out workItem, Timeout.Infinite))
        workItem.Key(workItem.Value);
    }

    public void Complete()
    {
      _queue.CompleteAdding();
    }

    public bool IsStopped()
    {
      return _queue.IsCompleted;
    }

    public int Length => _queue.Count;

    public static void Run(Func<Task> func)
    {
      var prevCtx = Current;
      try {
        var syncCtx = new SingleThreadSynchronizationContext();
        SetSynchronizationContext(syncCtx);

        var t = func();
        t.ContinueWith(
            delegate { syncCtx.Complete(); }, TaskScheduler.Default);

        syncCtx.RunOnCurrentThread();

        t.GetAwaiter().GetResult();
      }
      finally {
        SetSynchronizationContext(prevCtx);
      }
    }
  }
}