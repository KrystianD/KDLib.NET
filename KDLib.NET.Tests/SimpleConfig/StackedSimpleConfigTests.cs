using System;
using System.IO;
using KDLib.SimpleConfig;
using Xunit;

namespace KDLib.NET.Tests
{
  public class StackedSimpleConfigTests
  {
    private string YamlString1 = @"
root:
  val1: root-val1
  val2: root-val2
  path1: ./a.txt
  path2: ./b.txt
";

    private string YamlString2 = @"
custom:
  val1: custom-val1
  val3: custom-val3
  path1: ./c.txt
  path3: ./d.txt
";

    private string YamlString3 = @"
val1: third-val1
val4: third-val4
";

    [Fact]
    public void Test()
    {
      var cfg1 = YamlSimpleConfig.FromStringDir(YamlString1, "/cfg1");
      var cfg2 = YamlSimpleConfig.FromStringDir(YamlString2, "/cfg2");
      var cfg3 = YamlSimpleConfig.FromStringDir(YamlString3, "/cfg3");

      Assert.Equal("root-val1", cfg1.GetOption<string>("root.val1"));
      Assert.Equal("root-val2", cfg1.GetOption<string>("root.val2"));

      Assert.Equal("custom-val1", cfg2.GetOption<string>("custom.val1"));
      Assert.Equal("custom-val3", cfg2.GetOption<string>("custom.val3"));

      Assert.Equal("third-val1", cfg3.GetOption<string>("val1"));
      Assert.Equal("third-val4", cfg3.GetOption<string>("val4"));

      var stacked = new StackedSimpleConfig();
      stacked.AddConfig("root", cfg1);
      stacked.AddConfig("custom", cfg2);

      Assert.Equal("custom-val1", stacked.GetOption<string>("val1"));
      Assert.Equal("root-val2", stacked.GetOption<string>("val2"));

      var stacked2 = new StackedSimpleConfig();
      stacked2.AddConfig("root", cfg1);
      stacked2.AddConfig("custom", cfg2);
      stacked2.AddConfig(cfg3);

      Assert.Equal("third-val1", stacked2.GetOption<string>("val1"));
      Assert.Equal("root-val2", stacked2.GetOption<string>("val2"));
      Assert.Equal("custom-val3", stacked2.GetOption<string>("val3"));
      Assert.Equal("third-val4", stacked2.GetOption<string>("val4"));

      Assert.Equal("def", stacked2.GetOption<string>("val5", "def"));

      Assert.Equal("/cfg2/c.txt", stacked2.GetOptionAsPath("path1"));
      Assert.Equal("/cfg1/b.txt", stacked2.GetOptionAsPath("path2"));
      Assert.Equal("/cfg2/d.txt", stacked2.GetOptionAsPath("path3", "/a.txt"));
      Assert.Equal("/a.txt", stacked2.GetOptionAsPath("not_existing", "/a.txt"));

      Assert.Throws<ConfigOptionNotExistsException>(
          () => stacked2.GetOptionAsPath("not_existing"));
    }

    [Fact]
    public void TestError()
    {
      var stacked = new StackedSimpleConfig();
      var stacked2 = new StackedSimpleConfig();

      Assert.Throws<ArgumentException>(() => stacked.AddConfig(stacked2));
      
      Assert.Throws<InvalidOperationException>(() => stacked.ConfigDirectory);
    }
  }
}