using System;
using System.Collections.Generic;
using System.IO;
using KDLib.SimpleConfig;
using Xunit;

namespace KDLib.Tests
{
  public class YamlSimpleConfigTests
  {
    private string YamlString = @"
root:
  num: 2
  txt: ""A""
  path_rel: ./a.txt
  path_abs: /a.txt
  arr:
    - 1
    - 2
  obj:
    a: 1
    b: 2
  empty:
";

    [Fact]
    public void TestLoadString()
    {
      var cfg = (ISimpleConfig)YamlSimpleConfig.FromString(YamlString);

      var num = cfg.GetOption<int>("root.num");
      var numS = cfg.GetOption<string>("root.num");
      var txt = cfg.GetOption<string>("root.txt");
      var arr = cfg.GetOption<int[]>("root.arr");
      var obj = cfg.GetOption<Dictionary<string, int>>("root.obj");
      var emptyString = cfg.GetOption<string>("root.empty");
      var notExistingNull = cfg.GetOption<string>("root.not_existing", null);
      var notExistingDefault = cfg.GetOption<string>("root.not_existing", "def");

      Assert.Equal(2, num);
      Assert.Equal("2", numS);
      Assert.Equal("A", txt);
      Assert.Equal(new[] { 1, 2 }, arr);
      Assert.Equal(1, obj["a"]);
      Assert.Equal(2, obj["b"]);
      Assert.Equal("", emptyString);
      Assert.Null(notExistingNull);
      Assert.Equal("def", notExistingDefault);
    }

    [Fact]
    public void TestLoadFile()
    {
      var tmpPath = $"{Path.GetTempFileName()}.yaml";
      File.WriteAllText(tmpPath, YamlString);
      var cfg = (ISimpleConfig)YamlSimpleConfig.FromFile(tmpPath);
      File.Delete(tmpPath);

      var pathRel = cfg.GetOptionAsPath("root.path_rel");
      Assert.Equal("/tmp/a.txt", pathRel);

      var pathAbs = cfg.GetOptionAsPath("root.path_abs");
      Assert.Equal("/a.txt", pathAbs);

      var pathAbs1 = cfg.GetOptionAsPath("root.path_abs", "/default.txt");
      Assert.Equal("/a.txt", pathAbs1);

      var pathAbs2 = cfg.GetOptionAsPath("root.not_existing", "/default.txt");
      Assert.Equal("/default.txt", pathAbs2);
    }

    [Fact]
    public void TestError()
    {
      var cfg = YamlSimpleConfig.FromString(YamlString);

      Assert.Throws<ConfigOptionNotExistsException>(
          () => cfg.GetOption<int>("root.not_existing"));

      Assert.Throws<ConfigInvalidPathException>(
          () => cfg.GetOption<int>("root.num.a"));

      Assert.Throws<InvalidCastException>(
          () => cfg.GetOption<int>("root.txt"));

      Assert.Throws<InvalidOperationException>(
          () => cfg.GetOptionAsPath("root.path_abs"));
    }
  }
}