using System;
using System.IO;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public class BitWriter
  {
    private readonly Stream _stream;

    private byte _curByte;
    private int _curBitPos = 7;

    public BitWriter(Stream stream)
    {
      _stream = stream;
    }

    public void WriteBit(int value)
    {
      if (value > 0)
        _curByte |= (byte)(1 << _curBitPos);
      _curBitPos--;

      if (_curBitPos == -1)
        Flush();
    }

    private void WriteValue(ulong value, int typeByteSize, int length, string typeName)
    {
      const int ULongBitSize = 64;
      var typeBitLength = typeByteSize * 8;

      if (length > typeBitLength)
        throw new ArgumentOutOfRangeException(nameof(length), $"bit length must not be greater than {typeBitLength} for {typeName}");

      for (int i = 0; i < length; i++) {
        WriteBit((value & (1UL << (length - i - 1))) > 0 ? 1 : 0);
      }
    }

    public void Write(ulong value, int bits) => WriteValue(value, sizeof(ulong), bits, "ulong");
    public void Write(long value, int bits) => WriteValue((ulong)value, sizeof(long), bits, "long");
    public void Write(uint value, int bits) => WriteValue(value, sizeof(uint), bits, "uint");
    public void Write(int value, int bits) => WriteValue((ulong)value, sizeof(int), bits, "int");
    public void Write(ushort value, int bits) => WriteValue(value, sizeof(ushort), bits, "ushort");
    public void Write(short value, int bits) => WriteValue((ulong)value, sizeof(short), bits, "short");
    public void Write(byte value, int bits) => WriteValue(value, sizeof(byte), bits, "byte");
    public void Write(sbyte value, int bits) => WriteValue((ulong)value, sizeof(sbyte), bits, "sbyte");

    public void Flush()
    {
      if (_curBitPos == 7)
        return;

      _stream.WriteByte(_curByte);

      _curBitPos = 7;
      _curByte = 0;
    }
  }
}