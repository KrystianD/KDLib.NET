using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KDLib.Tests
{
  public class SynchronizationContextExtensionsTests
  {
    private static async Task Run(Func<SingleThreadSynchronizationContext, Task> fn)
    {
      SingleThreadSynchronizationContext ctx = null;
      bool[] stop = { false };

      var t = new Thread(() => {
        SingleThreadSynchronizationContext.Run(async () => {
          ctx = (SingleThreadSynchronizationContext)SynchronizationContext.Current;
          while (!stop[0])
            await Task.Delay(100);
        });
      });
      t.Start();

      await Task.Delay(10);

      await fn(ctx);

      stop[0] = true;
    }

    [Fact]
    public async void PostWaitAsyncTestOk()
    {
      await Run(async ctx => {
        var t2 = ctx.PostWaitAsync(async () => {
          await Task.Delay(10);
        });

        await Task.Delay(100);

        Assert.Equal(TaskStatus.RanToCompletion, t2.Status);
      });
    }

    [Fact]
    public async void PostWaitAsyncTestCancelled()
    {
      await Run(async ctx => {
        var t2 = ctx.PostWaitAsync(async () => {
          var tc = new CancellationTokenSource(10);
          await Task.Delay(100, tc.Token);
        });

        await Task.Delay(100);

        Assert.Equal(TaskStatus.Canceled, t2.Status);
      });
    }

    [Fact]
    public async void PostWaitAsyncTestFailed()
    {
      await Run(async ctx => {
        var t2 = ctx.PostWaitAsync(() => throw new Exception());

        await Task.Delay(100);

        Assert.Equal(TaskStatus.Faulted, t2.Status);
      });
    }
  }
}