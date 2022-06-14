using System;
using System.Collections.Generic;
using System.Linq;

namespace KDLib
{
  public static partial class LinqExtension
  {
    public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default)
    {
      var comparer = Comparer<TSource>.Default;
      using var en = source.GetEnumerator();

      if (en.MoveNext()) {
        var currentMin = en.Current;
        while (en.MoveNext()) {
          var current = en.Current;
          if (comparer.Compare(current, currentMin) < 0)
            currentMin = current;
        }

        return currentMin;
      }
      else {
        return defaultValue;
      }
    }

    public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue = default)
    {
      return source.Select(selector).MinOrDefault(defaultValue);
    }

    public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default)
    {
      var comparer = Comparer<TSource>.Default;
      using var en = source.GetEnumerator();

      if (en.MoveNext()) {
        var currentMax = en.Current;
        while (en.MoveNext()) {
          var current = en.Current;
          if (comparer.Compare(current, currentMax) > 0)
            currentMax = current;
        }

        return currentMax;
      }
      else {
        return defaultValue;
      }
    }

    public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue = default)
    {
      return source.Select(selector).MaxOrDefault(defaultValue);
    }
  }
}