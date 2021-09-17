using System;
using System.IO;
using Xunit;

namespace KDLib.Tests
{
  // ReSharper disable RedundantCast
  public class BitReaderTests
  {
    private static BitReader CreateReader(params byte[] data) => new(new MemoryStream(data));

    [Fact]
    public void TestByte()
    {
      var br = CreateReader(0b00111011);

      Assert.Equal(0, br.ReadBit());
      Assert.Equal(0, br.ReadBit());
      Assert.Equal(1, br.ReadBit());
      Assert.Equal(1, br.ReadBit());
      Assert.Equal(1, br.ReadBit());
      Assert.Equal(0, br.ReadBit());
      Assert.Equal(1, br.ReadBit());
      Assert.Equal(1, br.ReadBit());
    }

    [Fact]
    public void TestOutOfRange()
    {
      var br = CreateReader(0x00);

      for (int i = 0; i < 8; i++)
        br.ReadBit();

      Assert.Equal(-1, br.ReadBit());
    }

    [Fact]
    public void Test_TooLarge()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => CreateReader(0x00, 0x00).ReadBitAsInt8(10));
      Assert.Throws<ArgumentOutOfRangeException>(() => CreateReader(0x00, 0x00).ReadBitAsUInt8(10));
    }

    [Fact]
    public void Test_TooShortStream()
    {
      Assert.Throws<EndOfStreamException>(() => CreateReader(0x00, 0x00).ReadBitAsInt64(20));
    }

    [Fact]
    public void TestInt64_Length3()
    {
      Assert.Equal((long)2, CreateReader(0b010_00000).ReadBitAsInt64(3));
      Assert.Equal((long)1, CreateReader(0b001_00000).ReadBitAsInt64(3));
      Assert.Equal((long)0, CreateReader(0b000_00000).ReadBitAsInt64(3));
      Assert.Equal((long)-1, CreateReader(0b111_00000).ReadBitAsInt64(3));
      Assert.Equal((long)-2, CreateReader(0b110_00000).ReadBitAsInt64(3));
    }

    [Fact]
    public void TestUInt64_Length3()
    {
      Assert.Equal((ulong)2, CreateReader(0b010_00000).ReadBitAsUInt64(3));
      Assert.Equal((ulong)1, CreateReader(0b001_00000).ReadBitAsUInt64(3));
      Assert.Equal((ulong)0, CreateReader(0b000_00000).ReadBitAsUInt64(3));
      Assert.Equal((ulong)7, CreateReader(0b111_00000).ReadBitAsUInt64(3));
      Assert.Equal((ulong)6, CreateReader(0b110_00000).ReadBitAsUInt64(3));
    }

    [Fact]
    public void TestInt32_Length3()
    {
      Assert.Equal((int)2, CreateReader(0b010_00000).ReadBitAsInt32(3));
      Assert.Equal((int)1, CreateReader(0b001_00000).ReadBitAsInt32(3));
      Assert.Equal((int)0, CreateReader(0b000_00000).ReadBitAsInt32(3));
      Assert.Equal((int)-1, CreateReader(0b111_00000).ReadBitAsInt32(3));
      Assert.Equal((int)-2, CreateReader(0b110_00000).ReadBitAsInt32(3));
    }

    [Fact]
    public void TestUInt32_Length3()
    {
      Assert.Equal((uint)2, CreateReader(0b010_00000).ReadBitAsUInt32(3));
      Assert.Equal((uint)1, CreateReader(0b001_00000).ReadBitAsUInt32(3));
      Assert.Equal((uint)0, CreateReader(0b000_00000).ReadBitAsUInt32(3));
      Assert.Equal((uint)7, CreateReader(0b111_00000).ReadBitAsUInt32(3));
      Assert.Equal((uint)6, CreateReader(0b110_00000).ReadBitAsUInt32(3));
    }

    [Fact]
    public void TestInt16_Length3()
    {
      Assert.Equal((short)2, CreateReader(0b010_00000).ReadBitAsInt16(3));
      Assert.Equal((short)1, CreateReader(0b001_00000).ReadBitAsInt16(3));
      Assert.Equal((short)0, CreateReader(0b000_00000).ReadBitAsInt16(3));
      Assert.Equal((short)-1, CreateReader(0b111_00000).ReadBitAsInt16(3));
      Assert.Equal((short)-2, CreateReader(0b110_00000).ReadBitAsInt16(3));
    }

    [Fact]
    public void TestUInt16_Length3()
    {
      Assert.Equal((ushort)2, CreateReader(0b010_00000).ReadBitAsUInt16(3));
      Assert.Equal((ushort)1, CreateReader(0b001_00000).ReadBitAsUInt16(3));
      Assert.Equal((ushort)0, CreateReader(0b000_00000).ReadBitAsUInt16(3));
      Assert.Equal((ushort)7, CreateReader(0b111_00000).ReadBitAsUInt16(3));
      Assert.Equal((ushort)6, CreateReader(0b110_00000).ReadBitAsUInt16(3));
    }

    [Fact]
    public void TestInt8_Length3()
    {
      Assert.Equal((sbyte)2, CreateReader(0b010_00000).ReadBitAsInt8(3));
      Assert.Equal((sbyte)1, CreateReader(0b001_00000).ReadBitAsInt8(3));
      Assert.Equal((sbyte)0, CreateReader(0b000_00000).ReadBitAsInt8(3));
      Assert.Equal((sbyte)-1, CreateReader(0b111_00000).ReadBitAsInt8(3));
      Assert.Equal((sbyte)-2, CreateReader(0b110_00000).ReadBitAsInt8(3));
    }

    [Fact]
    public void TestUInt8_Length3()
    {
      Assert.Equal((byte)2, CreateReader(0b010_00000).ReadBitAsUInt8(3));
      Assert.Equal((byte)1, CreateReader(0b001_00000).ReadBitAsUInt8(3));
      Assert.Equal((byte)0, CreateReader(0b000_00000).ReadBitAsUInt8(3));
      Assert.Equal((byte)7, CreateReader(0b111_00000).ReadBitAsUInt8(3));
      Assert.Equal((byte)6, CreateReader(0b110_00000).ReadBitAsUInt8(3));
    }

    [Fact]
    public void TestInt64_Length9()
    {
      Assert.Equal((long)5, CreateReader(0b00000010, 0b1_0000000).ReadBitAsInt64(9));
      Assert.Equal((long)3, CreateReader(0b00000001, 0b1_0000000).ReadBitAsInt64(9));
      Assert.Equal((long)1, CreateReader(0b00000000, 0b1_0000000).ReadBitAsInt64(9));
      Assert.Equal((long)0, CreateReader(0b00000000, 0b0_0000000).ReadBitAsInt64(9));
      Assert.Equal((long)-1, CreateReader(0b11111111, 0b1_0000000).ReadBitAsInt64(9));
      Assert.Equal((long)-2, CreateReader(0b11111111, 0b0_0000000).ReadBitAsInt64(9));
    }

    [Fact]
    public void TestInt32_Length9()
    {
      Assert.Equal((int)5, CreateReader(0b00000010, 0b1_0000000).ReadBitAsInt32(9));
      Assert.Equal((int)3, CreateReader(0b00000001, 0b1_0000000).ReadBitAsInt32(9));
      Assert.Equal((int)1, CreateReader(0b00000000, 0b1_0000000).ReadBitAsInt32(9));
      Assert.Equal((int)0, CreateReader(0b00000000, 0b0_0000000).ReadBitAsInt32(9));
      Assert.Equal((int)-1, CreateReader(0b11111111, 0b1_0000000).ReadBitAsInt32(9));
      Assert.Equal((int)-2, CreateReader(0b11111111, 0b0_0000000).ReadBitAsInt32(9));
    }

    [Fact]
    public void TestInt16_Length9()
    {
      Assert.Equal((short)5, CreateReader(0b00000010, 0b1_0000000).ReadBitAsInt16(9));
      Assert.Equal((short)3, CreateReader(0b00000001, 0b1_0000000).ReadBitAsInt16(9));
      Assert.Equal((short)1, CreateReader(0b00000000, 0b1_0000000).ReadBitAsInt16(9));
      Assert.Equal((short)0, CreateReader(0b00000000, 0b0_0000000).ReadBitAsInt16(9));
      Assert.Equal((short)-1, CreateReader(0b11111111, 0b1_0000000).ReadBitAsInt16(9));
      Assert.Equal((short)-2, CreateReader(0b11111111, 0b0_0000000).ReadBitAsInt16(9));
    }
  }
}