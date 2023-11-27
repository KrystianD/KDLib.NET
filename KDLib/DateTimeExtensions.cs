using System;

namespace KDLib
{
  public static class DateTimeExtensions
  {
    private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

#if NET6_0_OR_GREATER
    public static DateOnly ToDateOnly(this DateTime dateTime) => new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
#endif

    public static DateTime WithMicroseconds(this DateTime d, int microseconds)
    {
      var date = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, d.Kind);
      date = new DateTime(date.Ticks + microseconds * TicksPerMicrosecond, date.Kind);
      return date;
    }

    public static DateTime WithMillisecond(this DateTime d, int milliseconds) => new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, milliseconds, d.Kind);
    public static DateTime WithSecond(this DateTime d, int second) => new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, second, d.Millisecond, d.Kind);
    public static DateTime WithMinute(this DateTime d, int minute) => new DateTime(d.Year, d.Month, d.Day, d.Hour, minute, d.Second, d.Millisecond, d.Kind);
    public static DateTime WithHour(this DateTime d, int hour) => new DateTime(d.Year, d.Month, d.Day, hour, d.Minute, d.Second, d.Millisecond, d.Kind);
    public static DateTime WithDay(this DateTime d, int day) => new DateTime(d.Year, d.Month, day, d.Hour, d.Minute, d.Second, d.Millisecond, d.Kind);
    public static DateTime WithMonth(this DateTime d, int month) => new DateTime(d.Year, month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, d.Kind);
    public static DateTime WithYear(this DateTime d, int year) => new DateTime(year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, d.Kind);
  }
}