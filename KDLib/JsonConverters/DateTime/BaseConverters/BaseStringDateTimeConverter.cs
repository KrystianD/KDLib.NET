using System;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime.BaseConverters
{
  public abstract class BaseStringDateTimeConverter : BaseDateTimeConverter
  {
    protected override System.DateTime? ParseFromValue(object value, Type objectType, bool isNullableType)
    {
      string str = (string)value;
      if (string.IsNullOrEmpty(str)) {
        if (!isNullableType)
          throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
        return null;
      }

      var result = ParseFromString(str);
      if (result is null && !isNullableType)
        throw new JsonSerializationException($"Cannot convert null value to {objectType}.");

      return result;
    }

    protected override object FormatToValue(System.DateTime datetime) => FormatToString(datetime);
    protected override JsonToken[] AllowedTokenTypes { get; } = { JsonToken.String };

    protected abstract System.DateTime? ParseFromString(string input);
    protected abstract string FormatToString(System.DateTime datetime);
  }
}