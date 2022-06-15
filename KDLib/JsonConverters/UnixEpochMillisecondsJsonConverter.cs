using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KDLib.JsonConverters
{
  [PublicAPI]
  public class UnixEpochMillisecondsJsonConverter : DateTimeConverterBase
  {
    private static bool IsGenericType(Type type)
    {
      return type.IsGenericType;
    }

    private static bool IsNullableType(Type t)
    {
      if (IsGenericType(t))
        return t.GetGenericTypeDefinition() == typeof(Nullable<>);
      return false;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      bool flag = IsNullableType(objectType);
      if (reader.TokenType == JsonToken.Null) {
        if (!flag)
          throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));
        return null;
      }

      Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
      if (reader.TokenType == JsonToken.Date) {
        if (type == typeof(DateTimeOffset)) {
          if (!(reader.Value is DateTimeOffset))
            return new DateTimeOffset((DateTime)reader.Value);
          return reader.Value;
        }

        object obj;
        if ((obj = reader.Value) is DateTimeOffset)
          return ((DateTimeOffset)obj).DateTime;
        return reader.Value;
      }

      if (reader.TokenType != JsonToken.Integer && reader.TokenType != JsonToken.String)
        throw new JsonSerializationException(string.Format("Unexpected token parsing date. Expected Integer or String, got {0}.", reader.TokenType));
      string str = reader.Value.ToString();
      if (string.IsNullOrEmpty(str) & flag)
        return null;

      if (type == typeof(DateTimeOffset)) {
        return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(str));
      }

      return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(str)).DateTime;
    }
  }
}