using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class ReflectionUtils
  {
    public static IList CreateListInstance(Type itemType)
    {
      return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
    }

    public static Type CreateNullableTypeFrom(Type type)
    {
      type = Nullable.GetUnderlyingType(type) ?? type;
      return type.IsValueType ? typeof(Nullable<>).MakeGenericType(type) : type;
    }
  }
}