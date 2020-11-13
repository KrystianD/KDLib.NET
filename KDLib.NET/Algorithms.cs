using System.Collections.Generic;

namespace KDLib
{
  public static class Algorithms
  {
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
  }
}