using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace KDLib.Tests
{
  // ReSharper disable RedundantCast
  public class BitWriterTests
  {
    private static byte[] CreateWriter(Action<BitWriter> x)
    {
      var ms = new MemoryStream();
      var bw = new BitWriter(ms);
      x(bw);
      bw.Flush();
      return ms.ToArray();
    }

    [Fact]
    public void TestPartialByte()
    {
      CreateWriter(x => {
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(0);
      }).Should().Equal(0b00111000);
    }

    [Fact]
    public void TestFullByte()
    {
      CreateWriter(x => {
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);
      }).Should().Equal(0b00111011);
    }

    [Fact]
    public void Test2PartialBytes()
    {
      CreateWriter(x => {
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);

        x.WriteBit(1);
      }).Should().Equal(0b00111011, 0b10000000);
    }

    [Fact]
    public void Test2FullBytes()
    {
      CreateWriter(x => {
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(1);
        x.WriteBit(0);
        x.WriteBit(1);
        x.WriteBit(1);

        x.WriteBit(1);
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(0);
        x.WriteBit(1);
      }).Should().Equal(0b00111011, 0b10000001);
    }

    [Fact]
    public void Test_TooLarge()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => CreateWriter(x => x.Write((int)1, 33)));
    }

    [Fact]
    public void TestUInt64_Length3()
    {
      CreateWriter(x => x.Write((ulong)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((ulong)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((ulong)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((ulong)7, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((ulong)6, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestInt32_Length3()
    {
      CreateWriter(x => x.Write((int)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((int)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((int)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((int)-1, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((int)-2, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestUInt32_Length3()
    {
      CreateWriter(x => x.Write((uint)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((uint)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((uint)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((uint)7, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((uint)6, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestInt16_Length3()
    {
      CreateWriter(x => x.Write((short)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((short)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((short)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((short)-1, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((short)-2, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestUInt16_Length3()
    {
      CreateWriter(x => x.Write((ushort)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((ushort)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((ushort)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((ushort)7, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((ushort)6, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestInt8_Length3()
    {
      CreateWriter(x => x.Write((sbyte)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((sbyte)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((sbyte)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((sbyte)-1, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((sbyte)-2, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestUInt8_Length3()
    {
      CreateWriter(x => x.Write((byte)2, 3)).Should().Equal(0b010_00000);
      CreateWriter(x => x.Write((byte)1, 3)).Should().Equal(0b001_00000);
      CreateWriter(x => x.Write((byte)0, 3)).Should().Equal(0b000_00000);
      CreateWriter(x => x.Write((byte)7, 3)).Should().Equal(0b111_00000);
      CreateWriter(x => x.Write((byte)6, 3)).Should().Equal(0b110_00000);
    }

    [Fact]
    public void TestInt64_Length9()
    {
      CreateWriter(x => x.Write((long)5, 9)).Should().Equal(0b00000010, 0b1_0000000);
      CreateWriter(x => x.Write((long)3, 9)).Should().Equal(0b00000001, 0b1_0000000);
      CreateWriter(x => x.Write((long)1, 9)).Should().Equal(0b00000000, 0b1_0000000);
      CreateWriter(x => x.Write((long)0, 9)).Should().Equal(0b00000000, 0b0_0000000);
      CreateWriter(x => x.Write((long)-1, 9)).Should().Equal(0b11111111, 0b1_0000000);
      CreateWriter(x => x.Write((long)-2, 9)).Should().Equal(0b11111111, 0b0_0000000);
    }

    [Fact]
    public void TestInt32_Length9()
    {
      CreateWriter(x => x.Write((int)5, 9)).Should().Equal(0b00000010, 0b1_0000000);
      CreateWriter(x => x.Write((int)3, 9)).Should().Equal(0b00000001, 0b1_0000000);
      CreateWriter(x => x.Write((int)1, 9)).Should().Equal(0b00000000, 0b1_0000000);
      CreateWriter(x => x.Write((int)0, 9)).Should().Equal(0b00000000, 0b0_0000000);
      CreateWriter(x => x.Write((int)-1, 9)).Should().Equal(0b11111111, 0b1_0000000);
      CreateWriter(x => x.Write((int)-2, 9)).Should().Equal(0b11111111, 0b0_0000000);
    }

    [Fact]
    public void TestInt16_Length9()
    {
      CreateWriter(x => x.Write((short)5, 9)).Should().Equal(0b00000010, 0b1_0000000);
      CreateWriter(x => x.Write((short)3, 9)).Should().Equal(0b00000001, 0b1_0000000);
      CreateWriter(x => x.Write((short)1, 9)).Should().Equal(0b00000000, 0b1_0000000);
      CreateWriter(x => x.Write((short)0, 9)).Should().Equal(0b00000000, 0b0_0000000);
      CreateWriter(x => x.Write((short)-1, 9)).Should().Equal(0b11111111, 0b1_0000000);
      CreateWriter(x => x.Write((short)-2, 9)).Should().Equal(0b11111111, 0b0_0000000);
    }
  }
}