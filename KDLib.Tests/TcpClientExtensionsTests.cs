using System;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace KDLib.Tests
{
#if !NET5_0 && !NET6_0
  public class TcpClientExtensionsTests
  {
    [Fact]
    public async void Connect()
    {
      var client = new TcpClient();

      Assert.Null(await Record.ExceptionAsync(() => client.ConnectAsync("8.8.8.8", 53)));
    }

    [Fact]
    public async void ConnectTimeout()
    {
      var client = new TcpClient();

      await Assert.ThrowsAsync<TimeoutException>(() => client.ConnectAsync("8.8.8.8", 53, TimeSpan.FromMilliseconds(2)));
    }

    [Fact]
    public async void ConnectCancelled()
    {
      var client1 = new TcpClient();
      var client2 = new TcpClient();

      var c = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
      await Assert.ThrowsAsync<OperationCanceledException>(() => client1.ConnectAsync("8.8.8.7", 53, c.Token));

      await Assert.ThrowsAsync<OperationCanceledException>(() => client2.ConnectAsync("8.8.8.7", 53, TimeSpan.FromMilliseconds(2000), c.Token));
    }
  }
#endif
}