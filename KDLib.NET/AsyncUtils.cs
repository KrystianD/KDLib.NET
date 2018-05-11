using System;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static class AsyncUtils
  {
    public static async Task<T> WaitFutureTimeout<T>(Task<T> task, TimeSpan timeout)
    {
      using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
        var completed = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completed == task) {
          timeoutCancellationTokenSource.Cancel();
          return await task;
        }
        else {
          throw new TimeoutException();
        }
      }
    }

    public static async Task WaitFutureTimeout(Task task, TimeSpan timeout)
    {
      using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
        var completed = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completed == task) {
          timeoutCancellationTokenSource.Cancel();
          await task;
        }
        else {
          throw new TimeoutException();
        }
      }
    }

    public static async Task<Tuple<T1, T2>> WaitAll<T1, T2>(Task<T1> t1, Task<T2> t2)
    {
      await Task.WhenAll(t1, t2);
      return Tuple.Create(t1.Result, t2.Result);
    }

    public static async Task<Tuple<T1, T2, T3>> WaitAll<T1, T2, T3>(Task<T1> t1, Task<T2> t2, Task<T3> t3)
    {
      await Task.WhenAll(t1, t2, t3);
      return Tuple.Create(t1.Result, t2.Result, t3.Result);
    }
  }
}