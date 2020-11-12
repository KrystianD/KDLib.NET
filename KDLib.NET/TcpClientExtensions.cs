using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static class TcpClientConnectAsyncExtensions
  {
    public static Task ConnectAsync(this TcpClient client, IPAddress address, int port, TimeSpan timeout)
      => client.ConnectAsync(address, port, timeout, CancellationToken.None);

    public static Task ConnectAsync(this TcpClient client, IPAddress address, int port, CancellationToken cancellationToken)
      => client.ConnectAsync(address, port, Timeout.InfiniteTimeSpan, cancellationToken);

    public static async Task ConnectAsync(this TcpClient client, IPAddress address, int port, TimeSpan timeout, CancellationToken cancellationToken)
    {
      var connectTask = client.ConnectAsync(address, port);

      var cancellationTaskSource = new TaskCompletionSource<bool>();

      using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
        using (cancellationToken.Register(() => cancellationTaskSource.SetResult(true))) {
          var timeoutTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
          var cancellationTask = cancellationTaskSource.Task;

          var completed = await Task.WhenAny(connectTask, timeoutTask, cancellationTask);

          timeoutCancellationTokenSource.Cancel();

          if (completed == connectTask) {
            if (completed.IsFaulted) {
              if (completed.Exception?.InnerException != null)
                throw completed.Exception.InnerException;
              else
                throw new Exception("unknown error");
            }
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

    public static Task ConnectAsync(this TcpClient client, string host, int port, TimeSpan timeout)
      => client.ConnectAsync(IPAddress.Parse(host), port, timeout);

    public static Task ConnectAsync(this TcpClient client, string host, int port, CancellationToken cancellationToken)
      => client.ConnectAsync(IPAddress.Parse(host), port, cancellationToken);

    public static Task ConnectAsync(this TcpClient client, string host, int port, TimeSpan timeout, CancellationToken cancellationToken)
      => client.ConnectAsync(IPAddress.Parse(host), port, timeout, cancellationToken);
  }
}