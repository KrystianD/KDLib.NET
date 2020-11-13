using Xunit;

namespace KDLib.NET.Tests
{
  public class AlgorithmsTests
  {
    [Fact]
    public void Combinations()
    {
      var a = new[] { 1, 2, 3 };
      var b = new[] { 11, 22 };
      var c = new[] { 111, 222, 333 };

      Assert.Collection(Algorithms.Combinations(a, b, c),
                        x => Assert.Equal(new[] { 1, 11, 111 }, x),
                        x => Assert.Equal(new[] { 1, 11, 222 }, x),
                        x => Assert.Equal(new[] { 1, 11, 333 }, x),
                        x => Assert.Equal(new[] { 1, 22, 111 }, x),
                        x => Assert.Equal(new[] { 1, 22, 222 }, x),
                        x => Assert.Equal(new[] { 1, 22, 333 }, x),
                        x => Assert.Equal(new[] { 2, 11, 111 }, x),
                        x => Assert.Equal(new[] { 2, 11, 222 }, x),
                        x => Assert.Equal(new[] { 2, 11, 333 }, x),
                        x => Assert.Equal(new[] { 2, 22, 111 }, x),
                        x => Assert.Equal(new[] { 2, 22, 222 }, x),
                        x => Assert.Equal(new[] { 2, 22, 333 }, x),
                        x => Assert.Equal(new[] { 3, 11, 111 }, x),
                        x => Assert.Equal(new[] { 3, 11, 222 }, x),
                        x => Assert.Equal(new[] { 3, 11, 333 }, x),
                        x => Assert.Equal(new[] { 3, 22, 111 }, x),
                        x => Assert.Equal(new[] { 3, 22, 222 }, x),
                        x => Assert.Equal(new[] { 3, 22, 333 }, x));
    }
  }
}