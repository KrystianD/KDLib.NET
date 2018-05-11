using System;
using System.Collections;
using System.Collections.Generic;

namespace KDLib
{
  public static class ReflectionUtils
  {
    public static IList CreateListInstance(Type itemType)
    {
      return (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
    }
  }
}