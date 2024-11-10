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

    public static T With<T>(this T value, Action<T> f)
    {
      f(value);
      return value;
    }

    public static async Task<T> WithAsync<T>(this Task<T> task, Action<T> f)
    {
      var value = await task;
      f(value);
      return value;
    }
  }
}