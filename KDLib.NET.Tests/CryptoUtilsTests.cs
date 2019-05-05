using KDLib.Crypto;
using Xunit;

namespace KDLib.NET.Tests
{
  public class CrpytoUtilsTests
  {
    [Fact]
    public void TestConstantTimeAreEqual()
    {
      var a = new byte[] { 1, 2 };
      var b = new byte[] { 1, 2 };
      var c = new byte[] { 1, 3 };
      var d = new byte[] { 1, 2, 3 };

      Assert.True(CryptoUtils.ConstantTimeAreEqual(a, b));
      Assert.False(CryptoUtils.ConstantTimeAreEqual(a, c));
      Assert.False(CryptoUtils.ConstantTimeAreEqual(a, d));
    }
  }
}