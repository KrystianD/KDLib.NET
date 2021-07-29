using Xunit;

namespace KDLib.Tests
{
  public class StringUtilsTests
  {
    [Fact]
    public void RandomTest()
    {
      var t1 = StringUtils.GenerateRandomString(5);
      var t2 = StringUtils.GenerateRandomString(5);

      Assert.Equal(5, t1.Length);
      Assert.NotEqual(t1, t2);
    }
  }
}