using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KDLib.NET.Tests
{
  public class RequestsTests
  {
    private readonly ITestOutputHelper _testOutputHelper;

    public RequestsTests(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TimeoutTest()
    {
      var resp = await Requests.Get("https://httpbin.org/delay/2", timeout: TimeSpan.FromSeconds(4));
      Assert.Equal(200, (int) resp.StatusCode);

      await Assert.ThrowsAsync<TimeoutException>(
          async () => await Requests.Get("https://httpbin.org/delay/10", timeout: TimeSpan.FromSeconds(3)));

      await Assert.ThrowsAsync<TimeoutException>(
          async () => await Requests.Get("https://httpbin.org/delay/10", timeout: TimeSpan.FromSeconds(6)));
    }

    [Fact]
    public async Task CancelTest()
    {
      var cts = new CancellationTokenSource();

      var resp2 = Requests.Get("https://httpbin.org/delay/10", timeout: TimeSpan.FromSeconds(20), cancellationToken: cts.Token);

      await Task.Delay(100);
      cts.Cancel();

      await Task.Delay(1);

      Assert.Equal(TaskStatus.Canceled, resp2.Status);

      await Assert.ThrowsAsync<TaskCanceledException>(async () => await resp2);
    }
  }
}