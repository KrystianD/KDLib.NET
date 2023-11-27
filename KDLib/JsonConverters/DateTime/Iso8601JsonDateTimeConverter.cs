using System;
using System.Globalization;
using JetBrains.Annotations;
using KDLib.JsonConverters.DateTime.BaseConverters;

namespace KDLib.JsonConverters.DateTime
{
  [PublicAPI]
  public class Iso8601UTCJsonDateTimeConverter : BaseStringDateTimeConverter
  {
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    public override bool CanRead => true;
    public override bool CanWrite => true;

    protected override System.DateTime ParseFromString(string input)
    {
      return System.DateTime.ParseExact(input, Format, CultureInfo.CurrentCulture);
    }

    protected override string FormatToString(System.DateTime datetime)
    {
      if (datetime.Kind != DateTimeKind.Utc)
        throw new ArgumentException("DateTime objects must use Kind=UTC");

      return datetime.ToString(Format, CultureInfo.CurrentCulture);
    }
  }
}