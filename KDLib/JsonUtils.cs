using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace KDLib
{
  [PublicAPI]
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

              if (options.RemoveFields != null && options.RemoveFields.Contains(propertyNameLower))
                jObject.Remove(propertyName);
              else if (options.ReplaceWithValue != null && options.ReplaceWithValue.TryGetValue(propertyNameLower, out var replacement))
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

    public static JArray CleanJArray(JArray obj)
    {
      var cloned = (JArray) obj.DeepClone();
      CleanJTokenReturnIsEmpty(cloned);
      return cloned;
    }

    public static JObject CleanJObject(JObject obj)
    {
      var cloned = (JObject) obj.DeepClone();
      CleanJTokenReturnIsEmpty(cloned);
      return cloned;
    }

    public static JToken CleanJToken(JToken obj)
    {
      var cloned = obj.DeepClone();
      CleanJTokenReturnIsEmpty(cloned);
      return cloned;
    }

    private static bool CleanJTokenReturnIsEmpty(JToken v)
    {
      switch (v) {
        case JValue jValue:
          if (jValue.Value == null)
            return true;

          switch (jValue.Value) {
            case string s:
              return string.IsNullOrWhiteSpace(s);
            default:
              return false;
          }

        case JObject jObject:
          jObject.Properties()
                 .Where(x => CleanJTokenReturnIsEmpty(x.Value))
                 .ToList()
                 .ForEach(x => jObject.Remove(x.Name));
          return jObject.Count == 0;
        case JArray jArray:
          for (int i = jArray.Count - 1; i >= 0; i--)
            if (CleanJTokenReturnIsEmpty(jArray[i]))
              jArray.RemoveAt(i);
          return jArray.Count == 0;
        default:
          return false;
      }
    }
  }
}