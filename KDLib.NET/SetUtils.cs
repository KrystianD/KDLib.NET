using System.Collections.Generic;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class SetUtils
  {
    public static HashSet<T> Union<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      newSet.UnionWith(set2);
      return newSet;
    }

    public static HashSet<T> Intersect<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      newSet.IntersectWith(set2);
      return newSet;
    }

    public static HashSet<T> Except<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      newSet.ExceptWith(set2);
      return newSet;
    }

    public static HashSet<T> SymmetricExcept<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      newSet.SymmetricExceptWith(set2);
      return newSet;
    }

    public static bool Equals<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.SetEquals(set2);
    }

    public static bool Overlaps<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.Overlaps(set2);
    }

    public static bool IsSubsetOf<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.IsSubsetOf(set2);
    }

    public static bool IsSupersetOf<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.IsSupersetOf(set2);
    }

    public static bool IsProperSubsetOf<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.IsProperSubsetOf(set2);
    }

    public static bool IsProperSupersetOf<T>(IEnumerable<T> set1, IEnumerable<T> set2)
    {
      var newSet = new HashSet<T>(set1);
      return newSet.IsProperSupersetOf(set2);
    }
  }
}