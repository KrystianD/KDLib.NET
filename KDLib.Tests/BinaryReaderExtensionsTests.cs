using System.IO;
using FluentAssertions;
using Xunit;

namespace KDLib.Tests
{
  public class BinaryReaderExtensionsTests
  {
    private static BinaryReader CreateReader() => new BinaryReader(new MemoryStream(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }));

    [Fact]
    public void Test()
    {
      CreateReader().ReadCharBE().Should().Be((char)0x01);
      CreateReader().ReadByteBE().Should().Be(0x01);
      CreateReader().ReadInt16BE().Should().Be(0x0102);
      CreateReader().ReadInt32BE().Should().Be(0x01020304);
      CreateReader().ReadInt64BE().Should().Be(0x0102030405060708);
      CreateReader().ReadUInt16BE().Should().Be(0x0102);
      CreateReader().ReadUInt32BE().Should().Be(0x01020304);
      CreateReader().ReadUInt64BE().Should().Be(0x0102030405060708);
      CreateReader().ReadBytesBE(8).Should().Equal(0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
    }
  }
}