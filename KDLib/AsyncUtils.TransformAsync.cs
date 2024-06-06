using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static partial class AsyncUtils
  {
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public static async Task<List<TOutput>> TransformAsync<TInput, TOutput>(IEnumerable<TInput> input, int maxRunningTasks,
                                                                            Func<TInput, Task<TOutput>> processor,
                                                                            TaskScheduler? taskScheduler = null,
                                                                            CancellationToken token = default)
    {
      using var semaphore = new SemaphoreSlim(maxRunningTasks);

      async Task<TOutput> ItemProcessorOnCurrentContext(TInput item)
      {
        await semaphore.WaitAsync(token);
        try {
          return await processor(item);
        }
        finally {
          semaphore.Release();
        }
      }

      async Task<TOutput> ItemProcessorOnTaskScheduler(TInput item)
      {
        await semaphore.WaitAsync(token);
        try {
          var task = await Task.Factory.StartNew(async () => await processor(item),
                                                 token,
                                                 TaskCreationOptions.None,
                                                 taskScheduler);
          return await task;
        }
        finally {
          semaphore.Release();
        }
      }

      var tasks = taskScheduler == null
          ? input.Select(ItemProcessorOnCurrentContext)
          : input.Select(ItemProcessorOnTaskScheduler);

      var res = await Task.WhenAll(tasks);

      return res.ToList();
    }

    public static Task<List<KeyValuePair<TInput, TOutput>>> TransformMapAsync<TInput, TOutput>(IEnumerable<TInput> input, int maxRunningTasks,
                                                                                               Func<TInput, Task<TOutput>> processor,
                                                                                               TaskScheduler? taskScheduler = null,
                                                                                               CancellationToken token = default)
      => TransformAsync(input: input,
                        maxRunningTasks: maxRunningTasks,
                        processor: async val => new KeyValuePair<TInput, TOutput>(val, await processor(val)),
                        taskScheduler: taskScheduler,
                        token: token);

    public static async Task<List<TOutput>> TransformInChunksAsync<TInput, TOutput>(IEnumerable<TInput> input, int itemsPerTask, int maxRunningTasks,
                                                                                    Func<IReadOnlyList<TInput>, Task<List<TOutput>>> processor,
                                                                                    TaskScheduler? taskScheduler = null,
                                                                                    CancellationToken token = default)
    {
      var chunks = input.Chunk(itemsPerTask);
      var res = await TransformAsync(input: chunks,
                                     maxRunningTasks: maxRunningTasks,
                                     processor: processor,
                                     taskScheduler: taskScheduler,
                                     token: token);
      return res.SelectMany(x => x).ToList();
    }
  }
}