using System;
using System.Linq;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime.BaseConverters
{
  public abstract class BaseDateTimeConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(System.DateTime) || objectType == typeof(System.DateTime?);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(FormatToValue((System.DateTime)value));
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

      if (!AllowedTokenTypes.Contains(reader.TokenType))
        throw new JsonSerializationException($"Unexpected token when parsing date. Expected {AllowedTokenTypes.Select(x => x.ToString()).JoinString(", ")}, got {reader.TokenType}.");

      return ParseFromValue(reader.Value, objectType, isNullableType);
    }

    protected abstract System.DateTime? ParseFromValue(object value, Type objectType, bool isNullableType);
    protected abstract object FormatToValue(System.DateTime datetime);
    protected abstract JsonToken[] AllowedTokenTypes { get; }

    private static bool IsGenericType(Type type) => type.IsGenericType;
    private static bool IsNullableType(Type t) => IsGenericType(t) && t.GetGenericTypeDefinition() == typeof(Nullable<>);
  }
}