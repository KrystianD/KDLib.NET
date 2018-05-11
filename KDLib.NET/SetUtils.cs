using System.Collections.Generic;

namespace KDLib
{
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
  }
}