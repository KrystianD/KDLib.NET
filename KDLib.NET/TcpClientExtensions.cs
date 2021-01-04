using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static class TcpClientConnectAsyncExtensions
  {
    public static Task ConnectAsync(this TcpClient client, string host, int port, TimeSpan timeout)
      => client.ConnectAsync(host, port, timeout, CancellationToken.None);

    public static Task ConnectAsync(this TcpClient client, string host, int port, CancellationToken cancellationToken)
      => client.ConnectAsync(host, port, Timeout.InfiniteTimeSpan, cancellationToken);

    public static async Task ConnectAsync(this TcpClient client, string host, int port, TimeSpan timeout, CancellationToken cancellationToken)
    {
      var connectTask = client.ConnectAsync(host, port);

      var cancellationTaskSource = new TaskCompletionSource<bool>();

      using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
        using (cancellationToken.Register(() => cancellationTaskSource.SetResult(true))) {
          var timeoutTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
          var cancellationTask = cancellationTaskSource.Task;

          var completed = await Task.WhenAny(connectTask, timeoutTask, cancellationTask);

          timeoutCancellationTokenSource.Cancel();

          if (completed == connectTask) {
            if (completed.IsFaulted)
              throw completed.Exception?.InnerException ?? new Exception("unknown error");
          }
          else if (completed == timeoutTask) {
            client.Close();
            throw new TimeoutException();
          }
          else if (completed == cancellationTask) {
            client.Close();
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
      }
    }

    public static Task ConnectAsync(this TcpClient client, IPAddress address, int port, TimeSpan timeout)
      => client.ConnectAsync(address.ToString(), port, timeout);

    public static Task ConnectAsync(this TcpClient client, IPAddress address, int port, CancellationToken cancellationToken)
      => client.ConnectAsync(address.ToString(), port, cancellationToken);

    public static Task ConnectAsync(this TcpClient client, IPAddress address, int port, TimeSpan timeout, CancellationToken cancellationToken)
      => client.ConnectAsync(address.ToString(), port, timeout, cancellationToken);
  }
}