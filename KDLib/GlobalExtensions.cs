using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class GlobalExtensions
  {
    public static R Let<T, R>(this T value, Func<T, R> f) => f(value);

    public static async Task<R> LetAsync<T, R>(this Task<T> task, Func<T, R> f)
    {
      var value = await task;
      return f(value);
    }
  }
}