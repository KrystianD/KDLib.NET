using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static async Task<List<TOutput>> TransformAsync<TInput, TOutput>(IEnumerable<TInput> input, int maxRunningTasks,
                                                                            Func<TInput, Task<TOutput>> processor,
                                                                            TaskScheduler taskScheduler = null)
    {
      if (taskScheduler == null)
        taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

      return await Task.Factory.StartNew(
          async () =>
          {
            using (var semaphore = new SemaphoreSlim(maxRunningTasks)) {
              var tasks = input.Select(async item =>
              {
                await semaphore.WaitAsync();
                try {
                  return await processor(item);
                }
                finally {
                  semaphore.Release();
                }
              });

              return (await Task.WhenAll(tasks)).ToList();
            }
          },
          CancellationToken.None,
          TaskCreationOptions.None,
          taskScheduler).Unwrap();
    }

    public static async Task<List<TOutput>> TransformManyAsync<TInput, TOutput>(IList<TInput> input, int itemsPerTask, int maxRunningTasks,
                                                                                Func<IEnumerable<TInput>, Task<IEnumerable<TOutput>>> processor,
                                                                                TaskScheduler taskScheduler = null)
    {
      var chunks = new List<IEnumerable<TInput>>();

      for (int i = 0; i < input.Count; i += itemsPerTask)
        chunks.Add(input.Skip(i).Take(itemsPerTask));

      var res = await TransformAsync<IEnumerable<TInput>, IEnumerable<TOutput>>(chunks, maxRunningTasks, processor,
                                                                                taskScheduler: taskScheduler);

      return res.SelectMany(x => x).ToList();
    }
  }
}