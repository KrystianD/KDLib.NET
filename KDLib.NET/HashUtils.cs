using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class HashUtils
  {
    public static string CalculateMD5Hash(string input)
    {
      using (MD5 hash = MD5.Create()) {
        byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        return StringUtils.BinaryToHex(data);
      }
    }

    public static string CalculateSHA256Hash(string input)
    {
      using (SHA256 hash = SHA256.Create()) {
        byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        return StringUtils.BinaryToHex(data);
      }
    }
  }
}