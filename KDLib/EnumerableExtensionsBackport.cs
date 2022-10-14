using System;
using System.Collections.Generic;

namespace KDLib
{
  public static class EnumerableExtensionsBackport
  {
#if !NET6_0_OR_GREATER
    // taken from .NET 6 sources
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
      using IEnumerator<TSource> e = source.GetEnumerator();

      while (e.MoveNext()) {
        TSource[] array = new TSource[size];
        array[0] = e.Current;
        int newSize;
        for (newSize = 1; newSize < array.Length && e.MoveNext(); ++newSize)
          array[newSize] = e.Current;
        if (newSize == array.Length) {
          yield return array;
        }
        else {
          Array.Resize<TSource>(ref array, newSize);
          yield return array;
          goto label_10;
        }
      }

    label_10: ;
    }
#endif
  }
}