using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  public static partial class AsyncUtils
  {
    [PublicAPI]
    public class SelectBuilder
    {
      private readonly Dictionary<Func<CancellationToken, Task>, Action<Task>> _tasks = new();

      public SelectBuilder Case(Func<CancellationToken, Task> task1, Action action)
      {
        _tasks[task1] = _ => action();
        return this;
      }

      public SelectBuilder Case<T>(Func<CancellationToken, Task<T>> task1, Action<T> action)
      {
        _tasks[task1] = x => action(((Task<T>)x).Result);
        return this;
      }

      public async Task WaitAsync(CancellationToken token = default)
      {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var taskToAction = _tasks.ToDictionary(x => x.Key(cts.Token), x => x.Value);

        var completedTask = await Task.WhenAny(taskToAction.Keys);
        cts.Cancel();
        token.ThrowIfCancellationRequested();

        taskToAction[completedTask](completedTask);
      }
    }

    [PublicAPI]
    public class SelectBuilder<TResult>
    {
      private readonly Dictionary<Func<CancellationToken, Task>, Func<Task, TResult>> _tasks = new();

      public SelectBuilder<TResult> Case(Func<CancellationToken, Task> task1, Func<TResult> action)
      {
        _tasks[task1] = _ => action();
        return this;
      }

      public SelectBuilder<TResult> Case<T>(Func<CancellationToken, Task<T>> task1, Func<T, TResult> action)
      {
        _tasks[task1] = x => action(((Task<T>)x).Result);
        return this;
      }

      public async Task<TResult> WaitAsync(CancellationToken token = default)
      {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var taskToAction = _tasks.ToDictionary(x => x.Key(cts.Token), x => x.Value);

        var completedTask = await Task.WhenAny(taskToAction.Keys);
        cts.Cancel();
        token.ThrowIfCancellationRequested();

        return taskToAction[completedTask](completedTask);
      }
    }

    public static SelectBuilder Select() => new();
    public static SelectBuilder<T> Select<T>() => new();
  }
}