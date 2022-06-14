using FluentAssertions;
using Xunit;

namespace KDLib.Tests.LinqTests
{
  public class MinMaxTests
  {
    [Fact]
    public void TestNotNullable()
    {
      var col = new[] { 2, 1, 2, 3 };

      col.MinOrDefault().Should().Be(1);
      col.MaxOrDefault().Should().Be(3);
      col.MinOrDefault(5).Should().Be(1);
      col.MaxOrDefault(5).Should().Be(3);

      col = new int[] { };

      col.MinOrDefault().Should().Be(0);
      col.MaxOrDefault().Should().Be(0);
      col.MinOrDefault(5).Should().Be(5);
      col.MaxOrDefault(5).Should().Be(5);
    }

    [Fact]
    public void TestNullable()
    {
      var col = new int?[] { 2, 1, 2, 3 };

      col.MinOrDefault().Should().Be(1);
      col.MaxOrDefault().Should().Be(3);
      col.MinOrDefault(5).Should().Be(1);
      col.MaxOrDefault(5).Should().Be(3);

      col = new int?[] { };

      col.MinOrDefault().Should().Be(null);
      col.MaxOrDefault().Should().Be(null);
      col.MinOrDefault(5).Should().Be(5);
      col.MaxOrDefault(5).Should().Be(5);
    }

    [Fact]
    public void TestFuncs()
    {
      var col = new int?[] { 2, 1, 2, 3 };

      col.MinOrDefault(x => x).Should().Be(1);
      col.MaxOrDefault(x => x).Should().Be(3);
      col.MinOrDefault(x => x, 5).Should().Be(1);
      col.MaxOrDefault(x => x, 5).Should().Be(3);
    }
  }
}