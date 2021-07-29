using System.Linq;
using System.Runtime.Serialization;
using Xunit;

namespace KDLib.Tests
{
  public class EnumTraitsTests
  {
    private enum TestEnum
    {
      [EnumMember(Value = "Value 1")]
      Value1 = -1,

      [EnumMember(Value = "Value 2")]
      Value2 = 2,

      [EnumMember(Value = "Value 3")]
      Value3,
    }

    [Fact]
    public void TestTraits()
    {
      Assert.Collection(EnumTraits<TestEnum>.EnumValues.OrderBy(x => (long)x),
                        x => Assert.Equal(TestEnum.Value1, x),
                        x => Assert.Equal(TestEnum.Value2, x),
                        x => Assert.Equal(TestEnum.Value3, x));

      Assert.False(EnumTraits<TestEnum>.IsEmpty);
      Assert.Equal(-1, EnumTraits<TestEnum>.MinValue);
      Assert.Equal(3, EnumTraits<TestEnum>.MaxValue);
    }

    [Fact]
    public void TestFindByMemberValue()
    {
      Assert.Equal(TestEnum.Value1, EnumTraits<TestEnum>.FindByMemberValue("Value 1"));
      Assert.Equal(TestEnum.Value2, EnumTraits<TestEnum>.FindByMemberValue("Value 2"));
      Assert.Null(EnumTraits<TestEnum>.FindByMemberValue("other"));
    }
  }
}