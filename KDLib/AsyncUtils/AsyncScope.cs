using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Nito.Disposables;

namespace KDLib
{
  [PublicAPI]
  public class AsyncScope<T>
  {
    private AsyncLocal<Stack<T>> _value = new();

    public T Value => _value.Value!.Peek();

    public AsyncScope(T def)
    {
      _value.Value = new Stack<T>();
      _value.Value!.Push(def);
    }

    public IDisposable Push(T value)
    {
      _value.Value!.Push(value);
      return Disposable.Create(() => _value.Value!.Pop());
    }
  }
}