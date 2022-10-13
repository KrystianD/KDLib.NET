using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using Xunit;

namespace KDLib.Tests
{
  public class BinaryWriterExtensionsTests
  {
    private static byte[] CreateWriter(Action<BinaryWriter> x)
    {
      var ms = new MemoryStream();
      var bw = new BinaryWriter(ms);
      x(bw);
      return ms.ToArray();
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void Test()
    {
      CreateWriter(x => x.WriteBE((char)0x01)).Should().Equal(0x01);
      CreateWriter(x => x.WriteBE((byte)0x01)).Should().Equal(0x01);
      CreateWriter(x => x.WriteBE((short)0x0102)).Should().Equal(0x01, 0x02);
      CreateWriter(x => x.WriteBE((int)0x01020304)).Should().Equal(0x01, 0x02, 0x03, 0x04);
      CreateWriter(x => x.WriteBE((long)0x0102030405060708)).Should().Equal(0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
      CreateWriter(x => x.WriteBE((ushort)0x0102)).Should().Equal(0x01, 0x02);
      CreateWriter(x => x.WriteBE((ushort)0x0102)).Should().Equal(0x01, 0x02);
      CreateWriter(x => x.WriteBE((uint)0x01020304)).Should().Equal(0x01, 0x02, 0x03, 0x04);
      CreateWriter(x => x.WriteBE((ulong)0x0102030405060708)).Should().Equal(0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
      CreateWriter(x => x.WriteBE(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 })).Should().Equal(0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08);
    }
  }
}