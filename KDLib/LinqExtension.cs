using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static partial class LinqExtension
  {
    public static string JoinString(this IEnumerable<string> source, string separator = "")
    {
      return string.Join(separator, source);
    }

    public static string JoinString(this IEnumerable<char> source, string separator = "")
    {
      return string.Join(separator, source);
    }

    public static bool In<T>(this T t, params T[] values)
    {
      return values.Contains(t);
    }

    public static bool In<T>(this T t, IEnumerable<T> values)
    {
      return values.Contains(t);
    }

    public static bool NotIn<T>(this T t, params T[] values)
    {
      return !values.Contains(t);
    }

    public static bool NotIn<T>(this T t, IEnumerable<T> values)
    {
      return !values.Contains(t);
    }

#if NETSTANDARD2_0 || NETSTANDARD2_1
    public static IEnumerable<(TFirst, TSecond)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
    {
      return first.Zip(second, (a, b) => (a, b));
    }
#endif

    public static Dictionary<TKey, List<TValue>> GroupByToDictionary<TValue, TKey>(this IEnumerable<TValue> seq, Func<TValue, TKey> key)
    {
      return seq.GroupBy(key).ToDictionary(x => x.Key, x => x.ToList());
    }

    public static Dictionary<TKey, List<TItem>> GroupByToDictionary<TValue, TKey, TItem>(this IEnumerable<TValue> seq, Func<TValue, TKey> key, Func<TValue, TItem> item)
    {
      return seq.GroupBy(key).ToDictionary(x => x.Key, x => x.Select(item).ToList());
    }

    public static Dictionary<TKey, int> GroupByToCount<TValue, TKey>(this IEnumerable<TValue> seq, Func<TValue, TKey> key)
    {
      return seq.GroupBy(key).ToDictionary(x => x.Key, x => x.Count());
    }

    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> list, Func<T, TKey> lookup)
    {
      return list.Distinct(new StructEqualityComparer<T, TKey>(lookup));
    }

    private class StructEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
      private readonly Func<T, TKey> _lookup;

      public StructEqualityComparer(Func<T, TKey> lookup)
      {
        _lookup = lookup;
      }

      public bool Equals(T x, T y) => _lookup(x).Equals(_lookup(y));
      public int GetHashCode(T obj) => _lookup(obj).GetHashCode();
    }

    public static IEnumerable<TSource> AggregatingTakeWhile<TSource, TAccumulate>(
        this IEnumerable<TSource> items,
        TAccumulate first,
        Func<TSource, TAccumulate, TAccumulate> aggregator,
        Func<TSource, TAccumulate, bool> predicate)
    {
      var accumulator = first;
      foreach (var item in items) {
        accumulator = aggregator(item, accumulator);
        if (!predicate(item, accumulator))
          yield break;
        yield return item;
      }
    }


    public static T Random<T>(this IReadOnlyList<T> list, Random random) => list[random.Next(list.Count)];
    public static T Random<T>(this IReadOnlyList<T> list) => list[ThreadSafeSharedRandom.RandInt(list.Count)];
  }
}