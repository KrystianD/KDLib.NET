using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static partial class AsyncUtils
  {
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

    public static async Task<List<TOutput>> TransformInChunksAsync<TInput, TOutput>(IEnumerable<TInput> input, int itemsPerTask, int maxRunningTasks,
                                                                                    Func<IReadOnlyList<TInput>, Task<List<TOutput>>> processor,
                                                                                    TaskScheduler taskScheduler = null)
    {
      var chunks = input.Chunk(itemsPerTask);
      var res = await TransformAsync(chunks, maxRunningTasks, processor, taskScheduler: taskScheduler);
      return res.SelectMany(x => x).ToList();
    }
  }
}