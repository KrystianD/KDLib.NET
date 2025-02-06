using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nito.AsyncEx;

namespace KDLib.RateLimiters;

[PublicAPI]
public class AsyncTokenBucketRateLimiter(int maxBucket, TimeSpan fillRate) : IAsyncRateLimiter
{
  private readonly int MaxBucket = maxBucket;

  private readonly AsyncMonitor _monitor = new();
  private readonly object _taskLock = new();

  private int _currentBucket = maxBucket;

  private CancellationTokenSource? _cts;

  public void Start()
  {
    lock (_taskLock) {
      if (_cts != null)
        return;

      _cts = new();
      Task.Run(Loop);
    }
  }

  private void Stop()
  {
    lock (_taskLock) {
      _cts?.Cancel();
      _cts = null;
    }
  }

  public async Task AcquireAsync(CancellationToken token = default)
  {
    using var _ = await _monitor.EnterAsync(token).ConfigureAwait(false);

    while (_currentBucket == 0) {
      await _monitor.WaitAsync(token).ConfigureAwait(false);
    }

    _currentBucket--;
  }

  // ReSharper disable once AsyncVoidMethod
  private async void Loop()
  {
    try {
      while (!_cts!.IsCancellationRequested) {
        using (await _monitor.EnterAsync(_cts.Token).ConfigureAwait(false)) {
          _currentBucket = Math.Min(_currentBucket + 1, MaxBucket);
          _monitor.Pulse();
        }

        await Task.Delay(fillRate, _cts.Token).ConfigureAwait(false);
      }
    }
    catch (OperationCanceledException) { }
  }
}