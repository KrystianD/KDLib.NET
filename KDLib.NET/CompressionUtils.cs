using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;

namespace KDLib
{
  public static class CompressionUtils
  {
    [PublicAPI]
    public static byte[] CompressGzip(byte[] data, CompressionLevel compressionLevel)
    {
      using var msOut = new MemoryStream();

      using (var cs = new GZipStream(msOut, compressionLevel, false))
        cs.Write(data, 0, data.Length);

      return msOut.ToArray();
    }

    [PublicAPI]
    public static byte[] DecompressGzip(byte[] data)
    {
      using var msIn = new MemoryStream(data);
      using var msOut = new MemoryStream();

      using (var cs = new GZipStream(msIn, CompressionMode.Decompress, false))
        cs.CopyTo(msOut);

      return msOut.ToArray();
    }
  }
}