using System;
using JetBrains.Annotations;
using KDLib.JsonConverters.DateTime.BaseConverters;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime
{
  [PublicAPI]
  public class JsonDoubleUtcEpochDateTimeConverter : BaseDateTimeConverter
  {
    private static readonly long EpochTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    private static readonly long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
    private static readonly long TicksPerMillisecond = TimeSpan.TicksPerMillisecond;
    private static readonly long TicksPerSecond = TimeSpan.TicksPerSecond;

    private readonly Mode _mode;

    [Flags]
    public enum Mode
    {
      AsSeconds = 1,
      AsMilliseconds = 2,
      AsMicroseconds = 4,
    }

    public JsonDoubleUtcEpochDateTimeConverter()
    {
      throw new Exception("must specify mode");
    }

    public JsonDoubleUtcEpochDateTimeConverter(Mode mode)
    {
      _mode = mode;

      var isValid = (mode & Mode.AsSeconds) == Mode.AsSeconds ||
                    (mode & Mode.AsMilliseconds) == Mode.AsMilliseconds ||
                    (mode & Mode.AsMicroseconds) == Mode.AsMicroseconds;
      if (!isValid) {
        throw new Exception("invalid mode");
      }
    }

    protected override System.DateTime? ParseFromValue(object value, Type objectType, bool isNullableType) => ParseFromDouble((double)value);

    protected override object FormatToValue(System.DateTime datetime)
    {
      if (_mode.HasFlag(Mode.AsSeconds)) {
        return (double)(datetime.Ticks - EpochTicks) / TicksPerSecond;
      }
      else if (_mode.HasFlag(Mode.AsMilliseconds)) {
        return (double)(datetime.Ticks - EpochTicks) / TicksPerMillisecond;
      }
      else if (_mode.HasFlag(Mode.AsMicroseconds)) {
        return (double)(datetime.Ticks - EpochTicks) / TicksPerMicrosecond;
      }
      else {
        throw new Exception("invalid mode");
      }
    }

    protected override JsonToken[] AllowedTokenTypes { get; } = { JsonToken.Integer, JsonToken.Float };

    private System.DateTime ParseFromDouble(double input)
    {
      System.DateTime date;
      if (_mode.HasFlag(Mode.AsSeconds)) {
        date = new System.DateTime(EpochTicks + (long)(input * TicksPerSecond));
      }
      else if (_mode.HasFlag(Mode.AsMilliseconds)) {
        date = new System.DateTime(EpochTicks + (long)(input * TicksPerMillisecond));
      }
      else if (_mode.HasFlag(Mode.AsMicroseconds)) {
        date = new System.DateTime(EpochTicks + (long)(input * TicksPerMicrosecond));
      }
      else {
        throw new Exception("invalid mode");
      }

      return System.DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
  }
}