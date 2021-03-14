using System.IO;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class BinaryWriterExtensions
  {
    public static void WriteBE(this BinaryWriter bw, short value) => bw.Write(BinaryUtils.SwapBytes(value));
    public static void WriteBE(this BinaryWriter bw, int value) => bw.Write(BinaryUtils.SwapBytes(value));
    public static void WriteBE(this BinaryWriter bw, long value) => bw.Write(BinaryUtils.SwapBytes(value));
    public static void WriteBE(this BinaryWriter bw, ushort value) => bw.Write(BinaryUtils.SwapBytes(value));
    public static void WriteBE(this BinaryWriter bw, uint value) => bw.Write(BinaryUtils.SwapBytes(value));
    public static void WriteBE(this BinaryWriter bw, ulong value) => bw.Write(BinaryUtils.SwapBytes(value));
  }
}