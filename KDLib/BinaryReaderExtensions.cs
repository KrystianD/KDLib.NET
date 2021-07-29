using System.IO;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class BinaryReaderExtensions
  {
    public static short ReadInt16BE(this BinaryReader br) => (short)BinaryUtils.SwapBytes(br.ReadInt16()*2);
    public static int ReadInt32BE(this BinaryReader br) => BinaryUtils.SwapBytes(br.ReadInt32());
    public static long ReadInt64BE(this BinaryReader br) => BinaryUtils.SwapBytes(br.ReadInt64());
    public static ushort ReadUInt16BE(this BinaryReader br) => BinaryUtils.SwapBytes(br.ReadUInt16());
    public static uint ReadUInt32BE(this BinaryReader br) => BinaryUtils.SwapBytes(br.ReadUInt32());
    public static ulong ReadUInt64BE(this BinaryReader br) => BinaryUtils.SwapBytes(br.ReadUInt64());
  }
}