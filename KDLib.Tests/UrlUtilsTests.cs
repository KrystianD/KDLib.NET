using System.Collections.Generic;
using Xunit;

namespace KDLib.Tests
{
  public class UrlUtilsTests
  {
    [Fact]
    public void CreateURLEmpty()
    {
      var url = UrlUtils.CreateUrl("http://test.com", (Dictionary<string, string>)null);
      Assert.Equal("http://test.com", url);
    }

    [Fact]
    public void CreateURLKeyValuePairList()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new List<KeyValuePair<string, string>> {
          new KeyValuePair<string, string>("key1", "value1")
      });

      Assert.Equal("http://test.com/?key1=value1", url);
    }

    [Fact]
    public void CreateURLDictionary()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new Dictionary<string, string>() {
          ["key1"] = "value1",
      });

      Assert.Equal("http://test.com/?key1=value1", url);
    }

    [Fact]
    public void CreateURLDictionaryHttps()
    {
      var url = UrlUtils.CreateUrl("https://test.com", new Dictionary<string, string>() {
          ["key1"] = "value1",
      });

      Assert.Equal("https://test.com/?key1=value1", url);
    }

    [Fact]
    public void CreateURLObject()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new {
          key1 = "value1",
      });

      Assert.Equal("http://test.com/?key1=value1", url);
    }

    [Fact]
    public void CreateURLObjectArray()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new {
          key1 = new[] { "value1", "value2" },
      });

      Assert.Equal("http://test.com/?key1[]=value1&key1[]=value2", url);
    }

    [Fact]
    public void CreateURLEscape()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new Dictionary<string, string>() {
          ["key1"] = "value1 value2",
      });

      Assert.Equal("http://test.com/?key1=value1%20value2", url);
    }

    [Fact]
    public void ExtendUrlEscape()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new Dictionary<string, string>() {
          ["key1"] = "value1 value2",
      });

      Assert.Equal("http://test.com/?key1=value1%20value2", url);

      url = UrlUtils.ExtendUrl(url, new Dictionary<string, string> {
          ["key2"] = "value2"
      });

      Assert.Equal("http://test.com/?key1=value1%20value2&key2=value2", url);

      url = UrlUtils.ExtendUrl(url, new Dictionary<string, string> {
          ["key1"] = "value1",
          ["key3"] = "value3",
      });

      Assert.Equal("http://test.com/?key1=value1&key2=value2&key3=value3", url);
    }

    [Fact]
    public void CreateURLKeyValuePairListMultiple()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new List<KeyValuePair<string, string>> {
          new KeyValuePair<string, string>("key1", "value1"),
          new KeyValuePair<string, string>("key1", "value2"),
      });

      Assert.Equal("http://test.com/?key1=value1&key1=value2", url);
    }

    [Fact]
    public void CreateURLKeyValuePairListMultipleArray()
    {
      var url = UrlUtils.CreateUrl("http://test.com", new List<KeyValuePair<string, string>> {
          new KeyValuePair<string, string>("key1[]", "value1"),
          new KeyValuePair<string, string>("key1[]", "value2"),
      });

      Assert.Equal("http://test.com/?key1[]=value1&key1[]=value2", url);
    }
  }
}