using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class SynchronizationContextExtensions
  {
    public static void Post(this SynchronizationContext ctx, Action d) => ctx.Post(_ => { d(); }, null);

    public static Task<T> PostWaitAsync<T>(this SynchronizationContext ctx, Func<Task<T>> d)
    {
      var tcs = new TaskCompletionSource<T>();
      ctx.Post(async _ => {
        try {
          var result = await d();
          tcs.SetResult(result);
        }
        catch (TaskCanceledException) {
          tcs.SetCanceled();
        }
        catch (Exception e) {
          tcs.SetException(e);
        }
      }, null);
      return tcs.Task;
    }

    public static Task PostWaitAsync(this SynchronizationContext ctx, Func<Task> d)
    {
      var tcs = new TaskCompletionSource<bool>();
      ctx.Post(async _ => {
        try {
          await d();
          tcs.SetResult(true);
        }
        catch (TaskCanceledException) {
          tcs.SetCanceled();
        }
        catch (Exception e) {
          tcs.SetException(e);
        }
      }, null);
      return tcs.Task;
    }

    public static Task PostWaitAsync(this SynchronizationContext ctx, Action d)
    {
      var tcs = new TaskCompletionSource<bool>();
      ctx.Post(_ => {
        try {
          d();
          tcs.SetResult(true);
        }
        catch (TaskCanceledException) {
          tcs.SetCanceled();
        }
        catch (Exception e) {
          tcs.SetException(e);
        }
      }, null);
      return tcs.Task;
    }
  }
}