using System.Collections.Generic;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class Algorithms
  {
    /// <summary>
    /// Generate all combinations of passed lists.
    /// </summary>
    public static IEnumerable<T[]> Combinations<T>(params IList<T>[] lists)
    {
      var indices = new int[lists.Length];
      var lastIdx = indices.Length - 1;

      while (true) {
        var output = new T[lists.Length];
        for (var i = 0; i < lists.Length; i++)
          output[i] = lists[i][indices[i]];
        yield return output;

        indices[lastIdx]++;
        for (int i = lastIdx; i >= 0; i--) {
          if (indices[i] < lists[i].Count)
            break;

          if (i == 0)
            yield break;

          indices[i] = 0;
          indices[i - 1]++;
        }
      }
    }

    /// <summary>
    /// Generate all combinations of passed lists. The list returned in the enumerable is mutated in the next iteration.
    /// </summary>
    public static IEnumerable<IReadOnlyList<T>> CombinationsFast<T>(params IList<T>[] lists)
    {
      var indices = new int[lists.Length];
      var lastIdx = indices.Length - 1;

      var output = new T[lists.Length];
      for (var i = 0; i < lists.Length; i++)
        output[i] = lists[i][indices[i]];

      while (true) {
        output[lastIdx] = lists[lastIdx][indices[lastIdx]];
        yield return output;

        indices[lastIdx]++;

        if (indices[lastIdx] < lists[lastIdx].Count)
          continue;

        for (int i = lastIdx - 1; i >= 0; i--) {
          indices[i + 1] = 0;
          output[i + 1] = lists[i + 1][indices[i + 1]];

          indices[i]++;

          if (indices[i] == lists[i].Count) {
            if (i == 0)
              yield break;
          }
          else {
            output[i] = lists[i][indices[i]];

            break;
          }
        }
      }
    }
  }
}