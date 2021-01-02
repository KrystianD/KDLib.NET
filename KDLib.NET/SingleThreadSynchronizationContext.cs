using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public class SingleThreadSynchronizationContext : SynchronizationContext
  {
    private readonly
        BlockingCollection<KeyValuePair<SendOrPostCallback, object>>
        _mQueue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

    public override void Post(SendOrPostCallback d, object state)
    {
      _mQueue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
    }

    public void RunOnCurrentThread()
    {
      KeyValuePair<SendOrPostCallback, object> workItem;
      while (_mQueue.TryTake(out workItem, Timeout.Infinite))
        workItem.Key(workItem.Value);
    }

    public void Complete()
    {
      _mQueue.CompleteAdding();
    }

    public bool IsStopped()
    {
      return _mQueue.IsCompleted;
    }

    public static void Run(Func<Task> func)
    {
      var prevCtx = SynchronizationContext.Current;
      try {
        var syncCtx = new SingleThreadSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(syncCtx);

        var t = func();
        t.ContinueWith(
            delegate { syncCtx.Complete(); }, TaskScheduler.Default);

        syncCtx.RunOnCurrentThread();

        t.GetAwaiter().GetResult();
      }
      finally {
        SynchronizationContext.SetSynchronizationContext(prevCtx);
      }
    }
  }
}