using System.Collections.Generic;
using FluentAssertions;
using KDLib.Collections;
using Xunit;

namespace KDLib.Tests.Collections
{
  public class BiMultiMapTests
  {
    [Fact]
    public void BiMultiMapForwardTest()
    {
      var map = new BiMultiMap<int, string>();

      map.Add(1, "val1-1");
      map.Add(1, "val1-2");

      map.ContainsKey(1).Should().BeTrue();
      map.ContainsValue("val1-1").Should().BeTrue();
      map.ContainsValue("val1-3").Should().BeFalse();
      map.GetForward(1).Should().Equal("val1-1", "val1-2");
      map.KeysMapCount.Should().Be(1);
      map.ValuesMapCount.Should().Be(2);
    }

    [Fact]
    public void BiMultiMapReverseTest()
    {
      var map = new BiMultiMap<int, string>();

      map.Add(1, "val1-1");
      map.Add(1, "val1-2");
      map.Add(2, "val1-1");

      map.GetForward(1).Should().Equal("val1-1", "val1-2");
      map.GetForward(2).Should().Equal("val1-1");

      map.GetReverse("val1-1").Should().Equal(1, 2);
      map.GetReverse("val1-2").Should().Equal(1);

      map.KeysMapCount.Should().Be(2);
      map.ValuesMapCount.Should().Be(2);
    }

    [Fact]
    public void BiMultiMapDeleteByKeyTest()
    {
      var map = new BiMultiMap<int, string>();

      map.Add(1, "val1-1");
      map.Add(1, "val1-2");

      map.KeysMapCount.Should().Be(1);
      map.ValuesMapCount.Should().Be(2);

      map.DeleteByKey(1);

      map.KeysMapCount.Should().Be(0);
      map.ValuesMapCount.Should().Be(0);

      map.Add(1, "val1-1");
      map.Add(2, "val1-2");

      map.KeysMapCount.Should().Be(2);
      map.ValuesMapCount.Should().Be(2);

      map.DeleteByKey(1);

      map.KeysMapCount.Should().Be(1);
      map.ValuesMapCount.Should().Be(1);

      map.DeleteByKey(2);

      map.KeysMapCount.Should().Be(0);
      map.ValuesMapCount.Should().Be(0);
    }

    [Fact]
    public void BiMultiMapDeleteByValueTest()
    {
      var map = new BiMultiMap<int, string>();

      map.Add(1, "val1-1");
      map.Add(2, "val1-1");

      map.KeysMapCount.Should().Be(2);
      map.ValuesMapCount.Should().Be(1);

      map.DeleteByValue("val1-1");

      map.KeysMapCount.Should().Be(0);
      map.ValuesMapCount.Should().Be(0);

      map.Add(1, "val1-1");
      map.Add(2, "val1-2");

      map.KeysMapCount.Should().Be(2);
      map.ValuesMapCount.Should().Be(2);

      map.DeleteByValue("val1-1");

      map.KeysMapCount.Should().Be(1);
      map.ValuesMapCount.Should().Be(1);

      map.DeleteByValue("val1-2");

      map.KeysMapCount.Should().Be(0);
      map.ValuesMapCount.Should().Be(0);
    }

    [Fact]
    public void NoKeyTest()
    {
      var map = new BiMultiMap<int, string>();

      map.ContainsKey(1).Should().BeFalse();
      map.GetForward(1).Should().BeEmpty();
    }

    [Fact]
    public void NoValueTest()
    {
      var map = new BiMultiMap<int, string>();

      map.ContainsValue("val").Should().BeFalse();
      map.GetReverse("val").Should().BeEmpty();
    }

    [Fact]
    public void DeleteMissingTest()
    {
      var map = new BiMultiMap<int, string>();

      Assert.Throws<KeyNotFoundException>(() => map.DeleteByKey(1));
      Assert.Throws<KeyNotFoundException>(() => map.DeleteByValue("val"));

      map.DeleteByKey(1, missingOk: true);
      map.DeleteByValue("val", missingOk: true);
    }
  }
}