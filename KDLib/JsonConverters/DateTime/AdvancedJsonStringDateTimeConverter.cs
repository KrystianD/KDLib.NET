using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using KDLib.JsonConverters.DateTime.BaseConverters;
using Newtonsoft.Json;

namespace KDLib.JsonConverters.DateTime
{
  [PublicAPI]
  public class AdvancedJsonDateTimeConverter : BaseStringDateTimeConverter
  {
    private readonly Mode _mode;
    private readonly string[] _formats;

    private readonly Regex _digitRegex = new Regex("[0-9]");

    [Flags]
    public enum Mode
    {
      SeparatorT = 1,
      SeparatorSpace = 2,
      WithSeconds = 4,
      WithFractional = 4,
      WithZ = 32,
      WithOffset = 64,
      WithZOrOffset = 1024,
      AsUnspecified = 128,
      AsUTC = 256,
      AllZerosAsNull = 512,
    }

    public override bool CanRead => true;
    public override bool CanWrite => false;

    public AdvancedJsonDateTimeConverter(Mode mode)
    {
      bool HasMoreThanOneFlag(params Mode[] flags) => flags.Count(x => mode.HasFlag(x)) > 1;

      if (mode.HasFlag(Mode.WithFractional))
        mode |= Mode.WithSeconds;

      if (HasMoreThanOneFlag(Mode.SeparatorT, Mode.SeparatorSpace))
        throw new Exception("invalid mode");
      if (HasMoreThanOneFlag(Mode.WithZ, Mode.WithOffset))
        throw new Exception("invalid mode");
      if (HasMoreThanOneFlag(Mode.AsUnspecified, Mode.AsUTC))
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

      if (_mode.HasFlag(Mode.WithFractional)) {
        // ReSharper disable StringLiteralTypo
        formatParts.Add(new[] { ".FFFFFFF" });
      }

      if (_mode.HasFlag(Mode.WithZ))
        formatParts.Add(new[] { "Z" });
      else if (_mode.HasFlag(Mode.WithOffset))
        formatParts.Add(new[] { "zzz" });
      else if (_mode.HasFlag(Mode.WithZOrOffset))
        formatParts.Add(new[] { "Z", "zzz" });

      _formats = Algorithms.Combinations(formatParts.ToArray()).Select(x => x.JoinString("")).ToArray();
    }

    private DateTimeOffset ParseInternal(string input)
    {
      try {
        return DateTimeOffset.ParseExact(input, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
      }
      catch (FormatException e) {
        throw new JsonSerializationException(e.Message);
      }
    }

    protected override System.DateTime? ParseFromString(string input)
    {
      if (_mode.HasFlag(Mode.AllZerosAsNull)) {
        if (_digitRegex.Matches(input).All(x => x.Value == "0")) {
          return null;
        }
      }
      
      DateTimeOffset dateOffset = ParseInternal(input);

      // workaroud: DateTimeOffset.ParseExact parses date without an offset (2345-10-20 12:34) as local dates with UTC variant off by local offset
      var date = (_mode.HasFlag(Mode.WithOffset) || _mode.HasFlag(Mode.WithZOrOffset)) ? dateOffset.UtcDateTime : dateOffset.DateTime;

      var targetKind = _mode.HasFlag(Mode.AsUTC) ? DateTimeKind.Utc : DateTimeKind.Unspecified;

      return System.DateTime.SpecifyKind(date, targetKind);
    }

    protected override string FormatToString(System.DateTime datetime)
    {
      throw new NotSupportedException();
    }
  }
}