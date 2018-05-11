using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KDLib
{
  public static class ReflectionExtensions
  {
    public static bool IsGenericList(this Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    public static bool IsGenericEumerable(this Type type)
    {
      return type.IsGenericType && 
             (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.ReflectedType == typeof(Enumerable));
    }

    public static bool IsGenericList(this PropertyInfo type)
    {
      return type.PropertyType.IsGenericList();
    }


    public static bool IsNullable(this Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static Type GetNullableInnerType(this Type type)
    {
      if (!IsNullable(type))
        throw new Exception("Type is not nullable");

      return type.GenericTypeArguments[0];
    }

    public static bool IsAnonymous(this Type type)
    {
      if (type.IsGenericType) {
        var d = type.GetGenericTypeDefinition();
        if (d.IsClass && d.IsSealed && d.Attributes.HasFlag(TypeAttributes.NotPublic)) {
          var attributes = d.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false);
          if (attributes != null && attributes.Length > 0) {
            return true;
          }
        }
      }

      return false;
    }
  }
}