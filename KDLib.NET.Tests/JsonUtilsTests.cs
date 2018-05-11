using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KDLib.NET.Tests
{
  public class JsonUtilsTests
  {
    [Fact]
    public void JsonSort()
    {
      JObject obj = JsonUtils.SortKeys(JObject.Parse(@"
{
  'b': 2,
  'a': {
    'c': 1,
    'a': [1, {'b':2,'a':2}],
  },
}
"));

      var serialized = obj.ToString(Formatting.Indented);
      Assert.Equal(@"{
  ""a"": {
    ""a"": [
      1,
      {
        ""a"": 2,
        ""b"": 2
      }
    ],
    ""c"": 1
  },
  ""b"": 2
}", serialized);
    }
    
    [Fact]
    public void JsonSortArray()
    {
      JArray obj = JsonUtils.SortKeys(JArray.Parse(@"
[1, {'b':2,'a':2}]
"));

      var serialized = obj.ToString(Formatting.None);
      Assert.Equal(@"[1,{""a"":2,""b"":2}]", serialized);
    }
    
    [Fact]
    public void JsonSortToken()
    {
      JToken obj = JsonUtils.SortKeys(JValue.Parse(@"32"));

      var serialized = obj.ToString(Formatting.None);
      Assert.Equal(@"32", serialized);
    }
  }
}