using System.Collections.Generic;
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

    public class SanitizerOptions
    {
      public HashSet<string> RemoveFields;
      public Dictionary<string, string> ReplaceWithValue;
    }

    public static JToken SanitizeObject(JToken obj, SanitizerOptions options)
    {
      void SanitizeObjectInternal(JToken innerObj)
      {
        switch (innerObj) {
          case JObject jObject:
            var props = jObject.Properties().Select(x => x.Name).ToList();

            foreach (var propertyName in props) {
              var propertyNameLower = propertyName.ToLower();

              if (options.RemoveFields.Contains(propertyNameLower))
                jObject.Remove(propertyName);
              else if (options.ReplaceWithValue.TryGetValue(propertyNameLower, out var replacement))
                jObject[propertyName] = replacement;
              else
                SanitizeObjectInternal(jObject[propertyName]);
            }

            break;

          case JArray jArray:
            foreach (var jToken in jArray)
              SanitizeObjectInternal(jToken);
            break;
        }
      }

      var clone = obj.DeepClone();
      SanitizeObjectInternal(clone);
      return clone;
    }
  }
}