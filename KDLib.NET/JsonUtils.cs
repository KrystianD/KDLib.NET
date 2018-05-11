using System.Linq;
using Newtonsoft.Json.Linq;

namespace KDLib
{
  public static class JsonUtils
  {
    public static T SortKeys<T>(T original) where T : JToken
    {
      switch (original) {
        case JObject value:
          return (T) (object) new JObject(value.Properties()
                                               .OrderBy(p => p.Name)
                                               .Select(x => new JProperty(x.Name, SortKeys(x.Value))));

        case JArray array:
          return (T) (object) new JArray(array.Select(SortKeys));

        default:
          return original;
      }
    }
  }
}