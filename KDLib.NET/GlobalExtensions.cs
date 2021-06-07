using System;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class GlobalExtensions
  {
    public static R Let<T, R>(this T value, Func<T, R> f) => f(value);
  }
}