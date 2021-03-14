using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class StreamExtensions
  {
    public static int ReadAll(this Stream s, byte[] buffer, int offset, int count)
    {
      int read = 0;
      while (read < count) {
        int rd = s.Read(buffer, offset + read, count - read);
        if (rd == 0)
          throw new EndOfStreamException();

        read += rd;
      }

      return read;
    }

    public static byte[] ReadAll(this Stream s, int count)
    {
      byte[] bytes = new byte[count];
      s.ReadAll(bytes, 0, count);
      return bytes;
    }

    public static Task<int> ReadAllAsync(this Stream s, byte[] buffer, int offset, int count) => s.ReadAllAsync(buffer, offset, count, CancellationToken.None);

    public static async Task<int> ReadAllAsync(this Stream s, byte[] buffer, int offset, int count, CancellationToken token)
    {
      int read = 0;
      while (read < count) {
        int rd = await s.ReadAsync(buffer, offset + read, count - read, token);
        if (rd == 0)
          throw new EndOfStreamException();

        read += rd;
      }

      return read;
    }

    public static async Task<byte[]> ReadAllAsync(this Stream s, int count, CancellationToken token)
    {
      byte[] bytes = new byte[count];
      await s.ReadAllAsync(bytes, 0, count, token);
      return bytes;
    }

    public static Task<byte[]> ReadAllAsync(this Stream s, int count) => s.ReadAllAsync(count, CancellationToken.None);

    public static T ReadStruct<T>(this Stream s)
    {
      byte[] bytes = new byte[BinaryUtils.StructSize<T>()];
      s.ReadAll(bytes, 0, bytes.Length);

      return BinaryUtils.StructFromBytes<T>(bytes);
    }

    public static Task<T> ReadStructAsync<T>(this Stream s) => s.ReadStructAsync<T>(CancellationToken.None);

    public static async Task<T> ReadStructAsync<T>(this Stream s, CancellationToken token)
    {
      byte[] bytes = new byte[BinaryUtils.StructSize<T>()];
      await s.ReadAllAsync(bytes, 0, bytes.Length, token);

      return BinaryUtils.StructFromBytes<T>(bytes);
    }

    public static void WriteStruct<T>(this Stream s, T obj)
    {
      var bytes = BinaryUtils.StructToBytes(obj);
      s.Write(bytes, 0, bytes.Length);
    }

    public static Task WriteStructAsync<T>(this Stream s, T obj) => s.WriteStructAsync(obj, CancellationToken.None);

    public static Task WriteStructAsync<T>(this Stream s, T obj, CancellationToken token)
    {
      var bytes = BinaryUtils.StructToBytes(obj);
      return s.WriteAsync(bytes, 0, bytes.Length, token);
    }
  }
}