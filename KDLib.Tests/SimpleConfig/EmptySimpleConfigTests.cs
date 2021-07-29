using System;
using KDLib.SimpleConfig;
using Xunit;

namespace KDLib.Tests
{
  public class EmptySimpleConfigTests
  {
    [Fact]
    public void TestEmpty()
    {
      var cfg = YamlSimpleConfig.Empty;
      Assert.Null(cfg.GetOption<string>("x", null));

      Assert.Throws<InvalidOperationException>(() => cfg.ConfigDirectory);
    }
  }
}