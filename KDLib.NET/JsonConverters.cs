using System;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KDLib
{
  namespace JsonConverters
  {
    [PublicAPI]
    public class DateFormatJsonConverter : IsoDateTimeConverter
    {
      public DateFormatJsonConverter(string format)
      {
        DateTimeFormat = format;
      }

      public DateFormatJsonConverter(string format, DateTimeStyles styles)
      {
        DateTimeFormat = format;
        DateTimeStyles = styles;
      }
    }

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

    [PublicAPI]
    public class DecimalJsonConverter : JsonConverter
    {
      public override bool CanRead => false;

      public override bool CanConvert(Type objectType)
      {
        return objectType == typeof(decimal) || objectType == typeof(decimal?);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
        throw new NotImplementedException();
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        writer.WriteValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
      }
    }

    [PublicAPI]
    public class AdvancedJsonDateTimeConverter : DateTimeConverterBase
    {
      private readonly Mode _mode;

      [Flags]
      public enum Mode
      {
        SeparatorT = 1,
        SeparatorSpace = 2,
        WithSeconds = 4,
        WithMilliseconds3 = 8,
        WithMilliseconds6 = 16,
        WithZ = 32,
        WithOffset = 64,
        AsUnspecified = 128,
        AsUTC = 256,
      }

      public override bool CanRead => true;
      public override bool CanWrite => false;

      public override bool CanConvert(Type objectType)
      {
        return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
      }

      public AdvancedJsonDateTimeConverter(Mode mode)
      {
        if (mode.HasFlag(Mode.SeparatorT) && mode.HasFlag(Mode.SeparatorSpace))
          throw new Exception("invalid mode");
        if (mode.HasFlag(Mode.WithMilliseconds3) && mode.HasFlag(Mode.WithMilliseconds6))
          throw new Exception("invalid mode");
        if ((mode.HasFlag(Mode.WithMilliseconds3) || mode.HasFlag(Mode.WithMilliseconds6)) && !mode.HasFlag(Mode.WithSeconds))
          throw new Exception("invalid mode");
        if (mode.HasFlag(Mode.WithZ) && mode.HasFlag(Mode.WithOffset))
          throw new Exception("invalid mode");
        if (mode.HasFlag(Mode.AsUnspecified) && mode.HasFlag(Mode.AsUTC))
          throw new Exception("invalid mode");

        _mode = mode;
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
            throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
          return null;
        }

        if (reader.TokenType == JsonToken.Date)
          throw new Exception("Disable dates parsing with \"new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }\"");

        if (reader.TokenType != JsonToken.String)
          throw new JsonSerializationException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");

        string str = (string)reader.Value;
        if (string.IsNullOrEmpty(str) & flag)
          return null;

        return Convert(str);
      }

      private DateTime Convert(string input)
      {
        string format = "yyyy-MM-dd";

        if (_mode.HasFlag(Mode.SeparatorT))
          format += "T";
        else if (_mode.HasFlag(Mode.SeparatorSpace))
          format += " ";

        format += "HH:mm";

        if (_mode.HasFlag(Mode.WithSeconds))
          format += ":ss";

        if (_mode.HasFlag(Mode.WithMilliseconds3))
          format += ".fff";
        else if (_mode.HasFlag(Mode.WithMilliseconds6))
          format += ".ffffff";

        if (_mode.HasFlag(Mode.WithZ))
          format += "Z";
        else if (_mode.HasFlag(Mode.WithOffset))
          format += "zzz";

        var dateOffset = DateTimeOffset.ParseExact(input, format, CultureInfo.InvariantCulture);

        // workaroud: DateTimeOffset.ParseExact parses date without an offset (2345-10-20 12:34) as local dates with UTC variant off by local offset
        var date = _mode.HasFlag(Mode.WithOffset) ? dateOffset.UtcDateTime : dateOffset.DateTime;

        var targetKind = _mode.HasFlag(Mode.AsUTC) ? DateTimeKind.Utc : DateTimeKind.Unspecified;

        return DateTime.SpecifyKind(date, targetKind);
      }

      private static bool IsGenericType(Type type) => type.IsGenericType;
      private static bool IsNullableType(Type t) => IsGenericType(t) && t.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
  }
}