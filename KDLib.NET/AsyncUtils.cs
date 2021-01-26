using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class AsyncUtils
  {
    public static async Task<T> WaitFutureTimeout<T>(Task<T> task, TimeSpan timeout)
    {
      using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
        var completed = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completed == task) {
          timeoutCancellationTokenSource.Cancel();
          return task.Result;
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
        }
        else {
          throw new TimeoutException();
        }
      }
    }

    public static async Task<T> FastCancellableTask<T>(Task<T> task, CancellationToken token)
    {
      var completed = await Task.WhenAny(task, Task.Delay(Timeout.InfiniteTimeSpan, token));
      if (completed == task) {
        return task.Result;
      }
      else {
        throw new TaskCanceledException();
      }
    }

    public static async Task FastCancellableTask(Task task, CancellationToken token)
    {
      var completed = await Task.WhenAny(task, Task.Delay(Timeout.InfiniteTimeSpan, token));
      if (completed == task) { }
      else {
        throw new TaskCanceledException();
      }
    }

    public static async Task<Tuple<T1, T2>> WaitAll<T1, T2>(
        Task<T1> t1, Task<T2> t2)
    {
      await Task.WhenAll(t1, t2);
      return Tuple.Create(t1.Result, t2.Result);
    }

    public static async Task<Tuple<T1, T2, T3>> WaitAll<T1, T2, T3>(
        Task<T1> t1, Task<T2> t2, Task<T3> t3)
    {
      await Task.WhenAll(t1, t2, t3);
      return Tuple.Create(t1.Result, t2.Result, t3.Result);
    }

    public static async Task<Tuple<T1, T2, T3, T4>> WaitAll<T1, T2, T3, T4>(
        Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4)
    {
      await Task.WhenAll(t1, t2, t3, t4);
      return Tuple.Create(t1.Result, t2.Result, t3.Result, t4.Result);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5>> WaitAll<T1, T2, T3, T4, T5>(
        Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4, Task<T5> t5)
    {
      await Task.WhenAll(t1, t2, t3, t4, t5);
      return Tuple.Create(t1.Result, t2.Result, t3.Result, t4.Result, t5.Result);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6>> WaitAll<T1, T2, T3, T4, T5, T6>(
        Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4, Task<T5> t5, Task<T6> t6)
    {
      await Task.WhenAll(t1, t2, t3, t4, t5, t6);
      return Tuple.Create(t1.Result, t2.Result, t3.Result, t4.Result, t5.Result, t6.Result);
    }

    public static async Task<Tuple<T1, T2, T3, T4, T5, T6, T7>> WaitAll<T1, T2, T3, T4, T5, T6, T7>(
        Task<T1> t1, Task<T2> t2, Task<T3> t3, Task<T4> t4, Task<T5> t5, Task<T6> t6, Task<T7> t7)
    {
      await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7);
      return Tuple.Create(t1.Result, t2.Result, t3.Result, t4.Result, t5.Result, t6.Result, t7.Result);
    }

    public static async Task<List<TOutput>> TransformAsync<TInput, TOutput>(IEnumerable<TInput> input, int maxRunningTasks,
                                                                            Func<TInput, Task<TOutput>> processor,
                                                                            TaskScheduler taskScheduler = null)
    {
      if (taskScheduler == null)
        taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

      return await Task.Factory.StartNew(
          async () => {
            using var semaphore = new SemaphoreSlim(maxRunningTasks);
            
            var tasks = input.Select(async item => {
              // ReSharper disable once AccessToDisposedClosure
              await semaphore.WaitAsync();
              try {
                return await processor(item);
              }
              finally {
                // ReSharper disable once AccessToDisposedClosure
                semaphore.Release();
              }
            });

            return (await Task.WhenAll(tasks)).ToList();
          },
          CancellationToken.None,
          TaskCreationOptions.None,
          taskScheduler).Unwrap();
    }

    public static Task<List<KeyValuePair<TInput, TOutput>>> TransformMapAsync<TInput, TOutput>(IEnumerable<TInput> input, int maxRunningTasks,
                                                                                               Func<TInput, Task<TOutput>> processor,
                                                                                               TaskScheduler taskScheduler = null)
    {
      return TransformAsync(input, maxRunningTasks,
                            async val => new KeyValuePair<TInput, TOutput>(val, await processor(val)),
                            taskScheduler);
    }

    public static async Task<List<TOutput>> TransformInChunksAsync<TInput, TOutput>(IList<TInput> input, int itemsPerTask, int maxRunningTasks,
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