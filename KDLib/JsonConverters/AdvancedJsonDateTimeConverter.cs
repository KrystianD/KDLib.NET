using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KDLib.JsonConverters
{
  [PublicAPI]
  public class AdvancedJsonDateTimeConverter : BaseDateTimeConverter
  {
    private readonly Mode _mode;
    private readonly string[] _formats;

    [Flags]
    public enum Mode
    {
      SeparatorT = 1,
      SeparatorSpace = 2,
      WithSeconds = 4,
      WithMilliseconds = 8,
      WithMicroseconds = 16,
      WithZ = 32,
      WithOffset = 64,
      AsUnspecified = 128,
      AsUTC = 256,
      WithRelaxedFractional = 512,
    }

    public override bool CanRead => true;
    public override bool CanWrite => false;

    public AdvancedJsonDateTimeConverter(Mode mode)
    {
      if (mode.HasFlag(Mode.WithMicroseconds))
        mode |= Mode.WithMilliseconds;
      if (mode.HasFlag(Mode.WithMilliseconds))
        mode |= Mode.WithSeconds;

      if (mode.HasFlag(Mode.SeparatorT) && mode.HasFlag(Mode.SeparatorSpace))
        throw new Exception("invalid mode");
      if (mode.HasFlag(Mode.WithZ) && mode.HasFlag(Mode.WithOffset))
        throw new Exception("invalid mode");
      if (mode.HasFlag(Mode.AsUnspecified) && mode.HasFlag(Mode.AsUTC))
        throw new Exception("invalid mode");

      _mode = mode;

      var formatParts = new List<string[]> {
          new[] { "yyyy-MM-dd" },
      };

      if (_mode.HasFlag(Mode.SeparatorT))
        formatParts.Add(new[] { "T" });
      else if (_mode.HasFlag(Mode.SeparatorSpace))
        formatParts.Add(new[] { " " });

      formatParts.Add(new[] { "HH:mm" });

      if (_mode.HasFlag(Mode.WithSeconds))
        formatParts.Add(new[] { ":ss" });

      if (_mode.HasFlag(Mode.WithMicroseconds)) {
        // ReSharper disable StringLiteralTypo
        if (_mode.HasFlag(Mode.WithRelaxedFractional))
          formatParts.Add(new[] { ".ffffff", ".fffff", ".ffff", ".fff", ".ff", ".f", "" });
        else
          formatParts.Add(new[] { ".ffffff" });
        // ReSharper enable StringLiteralTypo
      }
      else if (_mode.HasFlag(Mode.WithMilliseconds)) {
        // ReSharper disable StringLiteralTypo
        if (_mode.HasFlag(Mode.WithRelaxedFractional))
          formatParts.Add(new[] { ".fff", ".ff", ".f", "" });
        else
          formatParts.Add(new[] { ".fff" });
        // ReSharper enable StringLiteralTypo
      }

      if (_mode.HasFlag(Mode.WithZ))
        formatParts.Add(new[] { "Z" });
      else if (_mode.HasFlag(Mode.WithOffset))
        formatParts.Add(new[] { "zzz" });

      _formats = Algorithms.Combinations(formatParts.ToArray()).Select(x => x.JoinString("")).ToArray();
    }

    private DateTimeOffset ParseInternal(string input)
    {
      FormatException lastError = null;

      foreach (var format in _formats) {
        try {
          return DateTimeOffset.ParseExact(input, format, CultureInfo.InvariantCulture);
        }
        catch (FormatException e) {
          lastError = e;
        }
      }

      if (lastError != null)
        throw new JsonSerializationException(lastError.Message);

      throw new InvalidOperationException();
    }

    protected override DateTime ParseFromString(string input)
    {
      DateTimeOffset dateOffset = ParseInternal(input);

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