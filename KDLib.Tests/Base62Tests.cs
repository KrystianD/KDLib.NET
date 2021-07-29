using Xunit;

namespace KDLib.Tests
{
  public class Base62Tests
  {
    [Fact]
    public void Test()
    {
      var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      var encoded = Base62Converter.Encode(data);
      var decoded = Base62Converter.Decode(encoded);

      Assert.Equal(data, decoded);
    }
  }
}