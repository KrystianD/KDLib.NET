using System;
using KDLib.JsonConverters;
using KDLib.JsonConverters.DateTime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KDLib.Tests.JsonConverters
{
  public class AdvancedJsonDateTimeConverterTests
  {
    public class Model
    {
      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorSpace)]
      public DateTime date_space;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT)]
      public DateTime date_t;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds)]
      public DateTime date_t_seconds;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZ)]
      public DateTime date_t_seconds_z;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithFractional |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZ)]
      public DateTime date_t_seconds_ms3_z;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithFractional |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZ)]
      public DateTime date_t_seconds_ms6_z;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithFractional |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZ)]
      public DateTime date_t_seconds_ms7_z;


      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithOffset)]
      public DateTime date_t_seconds_tz1;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithSeconds |
                                                            AdvancedJsonDateTimeConverter.Mode.WithFractional |
                                                            AdvancedJsonDateTimeConverter.Mode.WithOffset)]
      public DateTime date_t_seconds_ms3_tz1;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.AsUTC)]
      public DateTime date_t_utc;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZ |
                                                            AdvancedJsonDateTimeConverter.Mode.AsUTC)]
      public DateTime date_t_utc_z;

      [JsonConverter(typeof(AdvancedJsonDateTimeConverter), AdvancedJsonDateTimeConverter.Mode.SeparatorT |
                                                            AdvancedJsonDateTimeConverter.Mode.WithZOrOffset |
                                                            AdvancedJsonDateTimeConverter.Mode.AsUTC)]
      public DateTime date_t_z_offset;
    }

    private static Model ParseJson(object data) =>
        JsonConvert.DeserializeObject<Model>(JToken.FromObject(data).ToString(), new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

    private static DateTime CreateDateTime(int year, int month, int days, int hours, int minutes, int seconds, double microseconds) =>
        DateTime.SpecifyKind(new DateTime(year, month, days, hours, minutes, seconds).AddTicks((int)(microseconds * 10)), DateTimeKind.Unspecified);

    [Fact]
    public void DateSpace()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_space = "2345-10-20 12:34",
      }).date_space);
    }

    [Fact]
    public void DateT()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_t = "2345-10-20T12:34",
      }).date_t);
    }

    [Fact]
    public void DateTSeconds()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56, 0), ParseJson(new {
          date_t_seconds = "2345-10-20T12:34:56",
      }).date_t_seconds);
    }

    [Fact]
    public void DateTSecondsZ()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56, 0), ParseJson(new {
          date_t_seconds_z = "2345-10-20T12:34:56Z",
      }).date_t_seconds_z);
    }

    [Fact]
    public void DateTSecondsFractionalZ()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56, 123000), ParseJson(new {
          date_t_seconds_ms3_z = "2345-10-20T12:34:56.123Z",
      }).date_t_seconds_ms3_z);

      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56, 123456), ParseJson(new {
          date_t_seconds_ms6_z = "2345-10-20T12:34:56.123456Z",
      }).date_t_seconds_ms6_z);

      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56, 123456.7), ParseJson(new {
          date_t_seconds_ms7_z = "2345-10-20T12:34:56.1234567Z",
      }).date_t_seconds_ms7_z);
    }

    [Fact]
    public void DateTSecondsTz()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12 + 2, 34, 56, 0), ParseJson(new {
          date_t_seconds_tz1 = "2345-10-20T12:34:56-02:00",
      }).date_t_seconds_tz1);
    }

    [Fact]
    public void DateTSecondsMs3Tz()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12 + 2, 34, 56, 123000), ParseJson(new {
          date_t_seconds_ms3_tz1 = "2345-10-20T12:34:56.123-02:00",
      }).date_t_seconds_ms3_tz1);
    }

    [Fact]
    public void DateTUTC()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_t_utc = "2345-10-20T12:34",
      }).date_t_utc);

      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_t_utc_z = "2345-10-20T12:34Z",
      }).date_t_utc_z);
    }

    [Fact]
    public void DateError()
    {
      Assert.Throws<JsonSerializationException>(() => ParseJson(new {
          date_t_utc = "2345-10-20 12:34", // invalid format
      }).date_t_utc);
    }

    [Fact]
    public void DateVariousTimezones()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_t_z_offset = "2345-10-20T12:34Z",
      }).date_t_z_offset);

      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 00, 0), ParseJson(new {
          date_t_z_offset = "2345-10-20T12:34+00:00",
      }).date_t_z_offset);

      Assert.Equal(CreateDateTime(2345, 10, 20, 12 + 2, 34, 00, 0), ParseJson(new {
          date_t_z_offset = "2345-10-20T12:34-02:00",
      }).date_t_z_offset);
    }
  }
}