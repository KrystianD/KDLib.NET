using System;
using KDLib.JsonConverters;
using KDLib.JsonConverters.DateTime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KDLib.Tests.JsonConverters
{
  public class JsonDoubleEpochDateTimeConverterTests
  {
    public class Model
    {
      [JsonConverter(typeof(JsonDoubleUtcEpochDateTimeConverter), JsonDoubleUtcEpochDateTimeConverter.Mode.AsSeconds)]
      public DateTime date_s;

      [JsonConverter(typeof(JsonDoubleUtcEpochDateTimeConverter), JsonDoubleUtcEpochDateTimeConverter.Mode.AsMilliseconds)]
      public DateTime date_ms;

      [JsonConverter(typeof(JsonDoubleUtcEpochDateTimeConverter), JsonDoubleUtcEpochDateTimeConverter.Mode.AsMicroseconds)]
      public DateTime date_us;
    }

    private static Model ParseJson(object data) =>
        JsonConvert.DeserializeObject<Model>(JToken.FromObject(data).ToString(), new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

    private static DateTime CreateDateTime(int year, int month, int days, int hours, int minutes, int seconds, int milliseconds = 0, int microseconds = 0) =>
        DateTime.SpecifyKind(new DateTime(year, month, days, hours, minutes, seconds), DateTimeKind.Utc).WithMillisecond(milliseconds).WithMicroseconds(microseconds);

    [Fact]
    public void DateParse()
    {
      AssertDateTimeEqual(CreateDateTime(1970, 1, 1, 0, 0, 1, 500, 0), ParseJson(new {
          date_s = 1.5,
      }).date_s);

      AssertDateTimeEqual(CreateDateTime(1970, 1, 1, 0, 0, 0, 2, 500), ParseJson(new {
          date_ms = 2.500,
      }).date_ms);

      AssertDateTimeEqual(CreateDateTime(1970, 1, 1, 0, 0, 0, 0, 4), ParseJson(new {
          date_us = 4.0,
      }).date_us);
    }

    [Fact]
    public void DateSerialize()
    {
      var date = CreateDateTime(1970, 1, 1, 0, 0, 1, 2, 3);
      var m = new Model() {
          date_s = date,
          date_ms = date,
          date_us = date,
      };
      var json = JsonConvert.SerializeObject(m);
      Assert.Equal("{\"date_s\":1.002003,\"date_ms\":1002.003,\"date_us\":1002003.0}", json);
    }

    private static void AssertDateTimeEqual(DateTime expected, DateTime actual) => Assert.Equal(expected.Ticks, actual.Ticks);
  }
}