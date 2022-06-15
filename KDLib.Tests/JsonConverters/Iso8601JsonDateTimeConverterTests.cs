using System;
using KDLib.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KDLib.Tests.JsonConverters
{
  public class Iso8601JsonDateTimeConverterTests
  {
    public class Model
    {
      [JsonConverter(typeof(Iso8601UTCJsonDateTimeConverter))]
      public DateTime date;
    }

    private static Model ParseJson(object data) =>
        JsonConvert.DeserializeObject<Model>(JToken.FromObject(data).ToString(), new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

    private static DateTime CreateDateTime(int year, int month, int days, int hours, int minutes, int seconds) =>
        DateTime.SpecifyKind(new DateTime(year, month, days, hours, minutes, seconds), DateTimeKind.Utc);

    [Fact]
    public void DateParse()
    {
      Assert.Equal(CreateDateTime(2345, 10, 20, 12, 34, 56), ParseJson(new {
          date = "2345-10-20T12:34:56Z",
      }).date);
    }

    [Fact]
    public void DateSerialize()
    {
      var date = CreateDateTime(2345, 10, 20, 12, 34, 56);
      var m = new Model() {
          date = date,
      };
      var json = JsonConvert.SerializeObject(m);
      Assert.Equal("{\"date\":\"2345-10-20T12:34:56Z\"}", json);
    }
  }
}