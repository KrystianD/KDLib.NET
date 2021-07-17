using System;
using System.Linq;
using Xunit;

namespace KDLib.NET.Tests
{
  // ReSharper disable RedundantCast
  public class BinaryUtilsTests
  {
    void AssertSwap(short v) => Assert.Equal(BitConverter.ToInt16(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(ushort v) => Assert.Equal(BitConverter.ToUInt16(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(int v) => Assert.Equal(BitConverter.ToInt32(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(uint v) => Assert.Equal(BitConverter.ToUInt32(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(long v) => Assert.Equal(BitConverter.ToInt64(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(ulong v) => Assert.Equal(BitConverter.ToUInt64(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(float v) => Assert.Equal(BitConverter.ToSingle(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));
    void AssertSwap(double v) => Assert.Equal(BitConverter.ToDouble(BitConverter.GetBytes(v).Reverse().ToArray()), BinaryUtils.SwapBytes(v));

    [Fact]
    public void SwapShortTest()
    {
      AssertSwap((short)1234);
      AssertSwap((short)-1234);

      AssertSwap((ushort)1234);

      AssertSwap((int)1234);
      AssertSwap((int)-1234);

      AssertSwap((uint)1234);

      AssertSwap((long)1234);
      AssertSwap((long)-1234);

      AssertSwap((ulong)1234);

      AssertSwap((float)1234.0f);
      AssertSwap((float)-1234.0f);

      AssertSwap((double)1234.0);
      AssertSwap((double)-1234.0);
    }

    private struct TestStruct
    {
      public short ValShort;
      public ushort ValUshort;
      public int ValInt;
      public uint ValUint;
      public long ValLong;
      public ulong ValUlong;
      public float ValFloat;
      private double _valDouble;
      
      public double ValDouble
      {
        get => _valDouble;
        init => _valDouble = value;
      }
    }

    [Fact]
    public void StructSwapTest()
    {
      var v = new TestStruct() {
          ValShort = -1234,
          ValUshort = 1234,
          ValInt = -1234,
          ValUint = 1234,
          ValLong = -1234,
          ValUlong = 1234,
          ValFloat = 1234.0f,
          ValDouble = 1234.0,
      };

      BinaryUtils.SwapStructureEndianness(ref v);

      Assert.Equal(BinaryUtils.SwapBytes((short)-1234), v.ValShort);
      Assert.Equal(BinaryUtils.SwapBytes((ushort)1234), v.ValUshort);
      Assert.Equal(BinaryUtils.SwapBytes((int)-1234), v.ValInt);
      Assert.Equal(BinaryUtils.SwapBytes((uint)1234), v.ValUint);
      Assert.Equal(BinaryUtils.SwapBytes((long)-1234), v.ValLong);
      Assert.Equal(BinaryUtils.SwapBytes((ulong)1234), v.ValUlong);
      Assert.Equal(BinaryUtils.SwapBytes((float)1234.0f), v.ValFloat);
      Assert.Equal(BinaryUtils.SwapBytes((double)1234.0), v.ValDouble);
    }
  }
}