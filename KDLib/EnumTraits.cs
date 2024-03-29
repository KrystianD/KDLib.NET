﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class EnumTraits<TEnum> where TEnum : struct, Enum
  {
    private static HashSet<TEnum> _valuesSet;
    private static Dictionary<string, TEnum> _memberValueToEnum;
    private static Dictionary<TEnum, string> _enumToMemberValue;

    static EnumTraits()
    {
      var type = typeof(TEnum);
      var underlyingType = Enum.GetUnderlyingType(type);

      EnumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
      _valuesSet = new HashSet<TEnum>(EnumValues);
      _memberValueToEnum = EnumValues
                           .Select(x => (x.GetMemberValue(), x))
                           .Distinct(x => x.Item1)
                           .ToDictionary(x => x.Item1, x => x.Item2);
      _enumToMemberValue = EnumValues
                           .Select(x => (x.GetMemberValue(), x))
                           .ToDictionary(x => x.Item2, x => x.Item1);

      var longValues =
          EnumValues
              .Select(v => Convert.ChangeType(v, underlyingType))
              .Select(Convert.ToInt64)
              .ToList();

      IsEmpty = longValues.Count == 0;
      if (!IsEmpty) {
        var sorted = longValues.OrderBy(v => v).ToList();
        MinValue = sorted.Min();
        MaxValue = sorted.Max();
      }
    }

    // ReSharper disable StaticMemberInGenericType
    public static bool IsEmpty { get; }
    public static long MinValue { get; }
    public static long MaxValue { get; }
    public static TEnum[] EnumValues { get; }
    // ReSharper restore StaticMemberInGenericType

    public static bool IsValid(TEnum value) => _valuesSet.Contains(value);

    public static TEnum? FindByMemberValue(string value) => _memberValueToEnum.TryGetValue(value, out var val) ? (TEnum?)val : null;
    public static string GetMemberValue(TEnum @enum) => _enumToMemberValue[@enum];
  }
}