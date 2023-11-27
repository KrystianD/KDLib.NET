using System;
using System.Globalization;
using JetBrains.Annotations;

namespace KDLib.JsonConverters
{
  [PublicAPI]
  public class Iso8601UTCJsonDateTimeConverter : BaseStringDateTimeConverter
  {
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    public override bool CanRead => true;
    public override bool CanWrite => true;

    protected override DateTime ParseFromString(string input)
    {
      return DateTime.ParseExact(input, Format, CultureInfo.CurrentCulture);
    }

    protected override string FormatToString(DateTime datetime)
    {
      if (datetime.Kind != DateTimeKind.Utc)
        throw new ArgumentException("Datetime objects must use Kind=UTC");

      return datetime.ToString(Format, CultureInfo.CurrentCulture);
    }
  }
}