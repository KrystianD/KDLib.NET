using System;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime.BaseConverters
{
  public abstract class BaseStringDateTimeConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(System.DateTime) || objectType == typeof(System.DateTime?);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(FormatToString((System.DateTime)value));
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
        throw new JsonSerializationException($"Unexpected token when parsing date. Expected String, got {reader.TokenType}.");

      string str = (string)reader.Value;
      if (string.IsNullOrEmpty(str) && isNullableType)
        return null;

      return ParseFromString(str);
    }

    protected abstract System.DateTime ParseFromString(string input);
    protected abstract string FormatToString(System.DateTime datetime);

    private static bool IsGenericType(Type type) => type.IsGenericType;
    private static bool IsNullableType(Type t) => IsGenericType(t) && t.GetGenericTypeDefinition() == typeof(Nullable<>);
  }
}