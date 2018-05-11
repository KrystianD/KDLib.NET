using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace KDLib
{
  public static class UrlUtils
  {
    public static string UrlEncodeKeyValuePairs(NameValueCollection queryData)
    {
      return UrlEncodeKeyValuePairs(queryData.AllKeys.Select(x => new KeyValuePair<string, string>(x, queryData.Get(x))));
    }

    public static string UrlEncodeKeyValuePairs(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
    {
      if (nameValueCollection == null)
        throw new ArgumentNullException(nameof(nameValueCollection));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> nameValue in nameValueCollection) {
        if (stringBuilder.Length > 0)
          stringBuilder.Append('&');
        stringBuilder.Append(UrlEscape(nameValue.Key).Replace("%5B%5D", "[]"));
        stringBuilder.Append('=');
        stringBuilder.Append(UrlEscape(nameValue.Value));
      }

      return stringBuilder.ToString();
    }

    public static string UrlEscape(string data)
    {
      return string.IsNullOrEmpty(data) ? string.Empty : Uri.EscapeDataString(data);
    }

    public static string CreateUrl(string url, IEnumerable<KeyValuePair<string, string>> queryData)
    {
      if (queryData == null)
        return url;
      var qb = new UriBuilder(url);
      qb.Query = UrlEncodeKeyValuePairs(queryData);
      if (qb.Uri.IsDefaultPort)
        qb.Port = -1;
      return qb.ToString();
    }

    public static string CreateUrl(string url, object queryData)
    {
      return CreateUrl(url, ObjectToKeyValuePairs(queryData));
    }

    public static string CreateUrl(string url, NameValueCollection queryData)
    {
      if (queryData == null)
        return url;
      var qb = new UriBuilder(url);
      qb.Query = UrlEncodeKeyValuePairs(queryData);
      if (qb.Uri.IsDefaultPort)
        qb.Port = -1;
      return qb.ToString();
    }

    public static string ExtendUrl(string url, NameValueCollection queryData)
    {
      return ExtendUrl(url, ObjectToKeyValuePairs(queryData));
    }

    public static string ExtendUrl(string url, IEnumerable<KeyValuePair<string, string>> newQueryData)
    {
      if (newQueryData == null)
        return url;

      var qs = HttpUtility.ParseQueryString(new Uri(url).Query);
      foreach (var (key, value) in newQueryData)
        qs.Set(key, value);

      return CreateUrl(url, qs);
    }

    public static void ParseUri(string uri, out string scheme, out string username, out string password,
                                out string host, out int port)
      => ParseUri(uri, out scheme, out username, out password, out host, out port, out _);

    public static void ParseUri(string uri, out string scheme, out string username, out string password,
                                out string host, out int port, out string path)
    {
      int idx = uri.LastIndexOf('@');
      if (idx != -1) {
        uri = uri.Substring(0, idx).Replace("@", "___AT___") + uri.Substring(idx);
      }

      var u = new Uri(uri);

      scheme = u.Scheme;
      username = u.GetUsername()?.Replace("___AT___", "@");
      password = u.GetPassword()?.Replace("___AT___", "@");
      host = u.Host;
      port = u.Port;
      path = u.AbsolutePath;
    }

    // helpers
    private static IEnumerable<KeyValuePair<string, string>> ObjectToKeyValuePairs(object obj)
    {
      foreach (var x in obj.GetType().GetProperties()) {
        var value = x.GetValue(obj);
        if (value is ICollection e) {
          foreach (var item in e)
            yield return new KeyValuePair<string, string>(x.Name + "[]", item.ToString());
        }
        else {
          yield return new KeyValuePair<string, string>(x.Name, value.ToString());
        }
      }
    }
  }
}