using System;
using System.IO;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public class BitReader
  {
    private readonly Stream _stream;

    private int _curByte;
    private int _curBitPos = -1;

    public BitReader(Stream stream)
    {
      _stream = stream;
    }

    public int ReadBit()
    {
      if (_curBitPos == -1) {
        _curBitPos = 7;
        _curByte = _stream.ReadByte();
        if (_curByte == -1)
          return -1;
      }

      return (_curByte & (1 << _curBitPos--)) > 0 ? 1 : 0;
    }

    private ulong FillValue(int length)
    {
      const int ULongBitSize = 64;

      ulong value = 0;

      for (int i = 0; i < length; i++) {
        int bit = ReadBit();
        if (bit == -1)
          throw new EndOfStreamException();

        if (bit == 1)
          value |= (1UL << (ULongBitSize - i - 1));
      }

      return value;
    }

    private ulong ReadBitAsUnsigned(int length, int typeByteSize, string typeName)
    {
      const int ULongBitSize = 64;
      var typeBitLength = typeByteSize * 8;

      if (length > typeBitLength)
        throw new ArgumentOutOfRangeException(nameof(length), $"bit length must not be greater than {typeBitLength} for {typeName}");

      return FillValue(length) >> (ULongBitSize - length);
    }

    private long ReadBitAsSigned(int length, int typeByteSize, string typeName)
    {
      const int LongBitSize = 64;
      var typeBitLength = typeByteSize * 8;

      if (length > typeBitLength)
        throw new ArgumentOutOfRangeException(nameof(length), $"bit length must not be greater than {typeBitLength} for {typeName}");

      return (long)FillValue(length) >> (LongBitSize - length); // use sign-extension when shifting to the right
    }

    public ulong ReadBitAsUInt64(int length) => ReadBitAsUnsigned(length, sizeof(ulong), "ulong");
    public long ReadBitAsInt64(int length) => ReadBitAsSigned(length, sizeof(long), "long");
    public uint ReadBitAsUInt32(int length) => (uint)ReadBitAsUnsigned(length, sizeof(uint), "uint");
    public int ReadBitAsInt32(int length) => (int)ReadBitAsSigned(length, sizeof(int), "int");
    public ushort ReadBitAsUInt16(int length) => (ushort)ReadBitAsUnsigned(length, sizeof(ushort), "ushort");
    public short ReadBitAsInt16(int length) => (short)ReadBitAsSigned(length, sizeof(short), "short");
    public byte ReadBitAsUInt8(int length) => (byte)ReadBitAsUnsigned(length, sizeof(byte), "byte");
    public sbyte ReadBitAsInt8(int length) => (sbyte)ReadBitAsSigned(length, sizeof(sbyte), "sbyte");
  }
}