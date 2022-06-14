using System.IO.Compression;
using System.Linq;
using System.Text;
using Xunit;

namespace KDLib.Tests
{
  public class CompressionUtilsTests
  {
    [Fact]
    public void CancellableTaskTest()
    {
      var s = Enumerable.Range(0, 100).Select(x => "test").JoinString();
      var data = Encoding.ASCII.GetBytes(s);

      var compressed = CompressionUtils.CompressGzip(data, CompressionLevel.Fastest);
      var compressedHash = HashUtils.CalculateHashHex(HashType.MD5, compressed);
      Assert.Equal("aeb490cdc02fb9a0be1d8e78f5caaeaa", compressedHash);

      var decompressed = CompressionUtils.DecompressGzip(compressed);

      Assert.Equal(data, decompressed);
    }
  }
}