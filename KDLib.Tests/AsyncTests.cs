using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KDLib.Tests
{
  public class AsyncTests
  {
    private readonly ITestOutputHelper _testOutputHelper;

    public AsyncTests(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task WaitFutureTimeoutTest()
    {
      var task1 = new Func<Task<int>>(async () => {
        await Task.Delay(500);
        return 2;
      });
      var task2 = new Func<Task>(async () => {
        await Task.Delay(500);
      });

      var r = await AsyncUtils.WaitFutureTimeout(task1(), TimeSpan.FromMilliseconds(2000));
      Assert.Equal(2, r);

      await AsyncUtils.WaitFutureTimeout(task2(), TimeSpan.FromMilliseconds(2000));

      await Assert.ThrowsAsync<TimeoutException>(async () => await AsyncUtils.WaitFutureTimeout(task1(), TimeSpan.FromMilliseconds(100)));
      await Assert.ThrowsAsync<TimeoutException>(async () => await AsyncUtils.WaitFutureTimeout(task2(), TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task CancellableTaskTest()
    {
      var task1 = new Func<Task<int>>(async () => {
        await Task.Delay(500);
        return 2;
      });
      var task2 = new Func<Task>(async () => {
        await Task.Delay(500);
      });

      var cts1 = new CancellationTokenSource();
      var cts2 = new CancellationTokenSource();

      var t1 = AsyncUtils.FastCancellableTask(task1(), cts1.Token);
      var t2 = AsyncUtils.FastCancellableTask(task2(), cts2.Token);

      await Task.Delay(100);
      cts1.Cancel();
      cts2.Cancel();

      await Task.Delay(1);

      Assert.Equal(TaskStatus.Canceled, t1.Status);
      Assert.Equal(TaskStatus.Canceled, t2.Status);
    }

    [Fact]
    public async Task TransformInChunksAsyncTest()
    {
      var input = new[] { 1, 2, 3, 4, 5 };

      var output = await AsyncUtils.TransformInChunksAsync(input, 3, 2, async ints => ints.Select(x => x * 2).ToList());

      Assert.Equal(new[] { 2, 4, 6, 8, 10 }, output);
    }
  }
}