using System;

namespace KDLib
{
  public static class DateTimeExtensions
  {
#if NET6_0_OR_GREATER
    public static DateOnly ToDateOnly(this DateTime dateTime) => new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
#endif
  }
}