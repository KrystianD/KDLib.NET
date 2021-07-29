using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KDLib.Tests
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

    [Fact]
    public void JsonSanitizer()
    {
      var opt = new JsonUtils.SanitizerOptions() {
          RemoveFields = new HashSet<string>() {
              "b",
              "f",
              "h",
          },
          ReplaceWithValue = new Dictionary<string, string>() {
              ["e"] = "***",
          }
      };

      var obj = JToken.FromObject(new {
          a = 1,
          b = 2,
          c = 3,
          d = new {
              e = 4,
              f = 5,
              g = new[] {
                  new {
                      h = 6,
                      i = 7,
                  },
                  new {
                      h = 8,
                      i = 9,
                  }
              }
          }
      });

      var sanitized = JsonUtils.SanitizeObject(obj, opt);

      var expected = JToken.FromObject(new {
          a = 1,
          c = 3,
          d = new {
              e = "***",
              g = new[] {
                  new {
                      i = 7,
                  },
                  new {
                      i = 9,
                  }
              }
          }
      });

      Assert.Equal(expected, sanitized);
    }

    [Fact]
    public void JsonCleaner()
    {
      var obj1 = JObject.FromObject(new {
          a = 1,
          b = (string) null,
          c = "",
          d = 2,
          e = new[] {
              new {
                  f = 3,
                  g = "",
                  h = new int[] { },
              }
          },
          i = new {
          }
      });
      var obj1cleaned = JsonUtils.CleanJObject(obj1);
      var obj1expected = JObject.FromObject(new {
          a = 1,
          d = 2,
          e = new[] {
              new {
                  f = 3,
              }
          }
      });

      Assert.Equal(obj1expected, obj1cleaned);


      var obj2 = JArray.FromObject(new object[] {
              new {
              },
              new {
                      f = 3,
                      g = "",
                      h = new int[] { },
              }
      });
      var obj2cleaned = JsonUtils.CleanJArray(obj2);
      var obj2expected = JArray.FromObject(new object[] {
              new {
                      f = 3,
              }
      });

      Assert.Equal(obj2expected, obj2cleaned);
    }
  }
}