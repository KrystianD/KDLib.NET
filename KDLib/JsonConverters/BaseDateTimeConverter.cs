using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace KDLib.JsonConverters
{
  public abstract class BaseDateTimeConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(FormatToString((DateTime)value));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      bool isNullableType = IsNullableType(objectType);
      if (reader.TokenType == JsonToken.Null) {
        if (!isNullableType)
          throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
        return null;
      }

      if (reader.TokenType == JsonToken.Date)
        throw new Exception("Disable dates parsing with \"new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }\"");

      if (reader.TokenType != JsonToken.String)
        throw new JsonSerializationException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");

      string str = (string)reader.Value;
      if (string.IsNullOrEmpty(str) && isNullableType)
        return null;

      return ParseFromString(str);
    }

    protected abstract DateTime ParseFromString(string input);
    protected abstract string FormatToString(DateTime datetime);

    private static bool IsGenericType(Type type) => type.IsGenericType;
    private static bool IsNullableType(Type t) => IsGenericType(t) && t.GetGenericTypeDefinition() == typeof(Nullable<>);
  }
}