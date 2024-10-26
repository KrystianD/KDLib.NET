using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Nito.Disposables;

namespace KDLib
{
  [PublicAPI]
  public class AsyncScope<T>(T defaultValue)
  {
    private AsyncLocal<Stack<T>> _value = new();

    private Stack<T>? Stack
    {
      get => _value.Value!;
      set => _value.Value = value!;
    }

    public T Value => Stack == null || Stack.Count == 0 ? defaultValue : Stack.Peek();

    public IDisposable Push(T value)
    {
      Stack ??= new Stack<T>();
      Stack.Push(value);
      return Disposable.Create(() => Stack.Pop());
    }
  }
}