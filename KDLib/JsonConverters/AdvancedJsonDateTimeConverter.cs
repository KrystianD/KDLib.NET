using System;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KDLib.JsonConverters
{
  [PublicAPI]
  public class AdvancedJsonDateTimeConverter : BaseDateTimeConverter
  {
    private readonly Mode _mode;
    private readonly string _format;

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

      _format = format;
    }

    protected override DateTime ParseFromString(string input)
    {
      DateTimeOffset dateOffset;
      try {
        dateOffset = DateTimeOffset.ParseExact(input, _format, CultureInfo.InvariantCulture);
      }
      catch (FormatException e) {
        throw new JsonSerializationException(e.Message);
      }

      // workaroud: DateTimeOffset.ParseExact parses date without an offset (2345-10-20 12:34) as local dates with UTC variant off by local offset
      var date = _mode.HasFlag(Mode.WithOffset) ? dateOffset.UtcDateTime : dateOffset.DateTime;

      var targetKind = _mode.HasFlag(Mode.AsUTC) ? DateTimeKind.Utc : DateTimeKind.Unspecified;

      return DateTime.SpecifyKind(date, targetKind);
    }

    protected override string FormatToString(DateTime datetime)
    {
      throw new NotSupportedException();
    }
  }
}