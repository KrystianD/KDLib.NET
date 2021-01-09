using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  public static class MiscUtils
  {
    [PublicAPI]
    public static Task WaitForAndLog<T>(Func<T> value, Func<T, bool> condition, Action<T> logAction, TimeSpan? checkInterval = null, TimeSpan? logInterval = null)
    {
      return WaitForAndLog(() => Task.FromResult(value()), condition, logAction, checkInterval, logInterval);
    }

    [PublicAPI]
    public static async Task WaitForAndLog<T>(Func<Task<T>> value, Func<T, bool> condition, Action<T> logAction, TimeSpan? checkInterval = null, TimeSpan? logInterval = null)
    {
      checkInterval ??= TimeSpan.FromMilliseconds(100);
      logInterval ??= TimeSpan.FromMilliseconds(1000);

      int logEvery = (int)logInterval.Value.TotalMilliseconds / (int)checkInterval.Value.TotalMilliseconds;

      for (int i = 0;; i++) {
        var currentValue = await value();
        if (condition(currentValue))
          return;

        if (i % logEvery == 0)
          logAction(currentValue);

        await Task.Delay(checkInterval.Value);
      }
    }
  }
}