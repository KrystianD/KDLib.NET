using System;
using JetBrains.Annotations;
using KDLib.JsonConverters.DateTime.BaseConverters;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime
{
  [PublicAPI]
  public class JsonIntegerUtcEpochDateTimeConverter : BaseDateTimeConverter
  {
    private static readonly long EpochTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
    private const long TicksPerMillisecond = TimeSpan.TicksPerMillisecond;
    private const long TicksPerSecond = TimeSpan.TicksPerSecond;

    private readonly Mode _mode;

    [Flags]
    public enum Mode
    {
      AsSeconds = 1,
      AsMilliseconds = 2,
      AsMicroseconds = 4,
    }

    public JsonIntegerUtcEpochDateTimeConverter()
    {
      throw new Exception("must specify mode");
    }

    public JsonIntegerUtcEpochDateTimeConverter(Mode mode)
    {
      _mode = mode;

      var isValid = (mode & Mode.AsSeconds) == Mode.AsSeconds ||
                    (mode & Mode.AsMilliseconds) == Mode.AsMilliseconds ||
                    (mode & Mode.AsMicroseconds) == Mode.AsMicroseconds;
      if (!isValid) {
        throw new Exception("invalid mode");
      }
    }

    protected override System.DateTime? ParseFromValue(object value, Type objectType, bool isNullableType) => ParseFromLong((long)value);

    protected override object FormatToValue(System.DateTime datetime)
    {
      if (_mode.HasFlag(Mode.AsSeconds)) {
        return (datetime.Ticks - EpochTicks) / TicksPerSecond;
      }
      else if (_mode.HasFlag(Mode.AsMilliseconds)) {
        return (datetime.Ticks - EpochTicks) / TicksPerMillisecond;
      }
      else if (_mode.HasFlag(Mode.AsMicroseconds)) {
        return (datetime.Ticks - EpochTicks) / TicksPerMicrosecond;
      }
      else {
        throw new Exception("invalid mode");
      }
    }

    protected override JsonToken[] AllowedTokenTypes { get; } = { JsonToken.Integer };

    private System.DateTime ParseFromLong(long input)
    {
      System.DateTime date;
      if (_mode.HasFlag(Mode.AsSeconds)) {
        date = new System.DateTime(EpochTicks + input * TicksPerSecond);
      }
      else if (_mode.HasFlag(Mode.AsMilliseconds)) {
        date = new System.DateTime(EpochTicks + input * TicksPerMillisecond);
      }
      else if (_mode.HasFlag(Mode.AsMicroseconds)) {
        date = new System.DateTime(EpochTicks + input * TicksPerMicrosecond);
      }
      else {
        throw new Exception("invalid mode");
      }

      return System.DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
  }
}