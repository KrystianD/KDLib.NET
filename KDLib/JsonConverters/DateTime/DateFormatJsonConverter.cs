using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json.Converters;

namespace KDLib.JsonConverters.DateTime
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
}