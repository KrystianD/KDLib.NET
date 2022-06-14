using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace KDLib
{
  public enum HashType
  {
    MD5,
    SHA1,
    SHA256,
    SHA384,
    SHA512,
  }

  [PublicAPI]
  public static class HashUtils
  {
    // MD5
    public static byte[] CalculateHash(HashType hashType, byte[] input)
    {
      using HashAlgorithm hash = hashType switch {
          HashType.MD5 => MD5.Create(),
          HashType.SHA1 => SHA1.Create(),
          HashType.SHA256 => SHA256.Create(),
          HashType.SHA384 => SHA384.Create(),
          HashType.SHA512 => SHA512.Create(),
          _ => throw new ArgumentException("Invalid hash type"),
      };
      return hash!.ComputeHash(input);
    }

    public static byte[] CalculateHash(HashType hashType, string input) => CalculateHash(hashType, Encoding.UTF8.GetBytes(input));

    public static string CalculateHashHex(HashType hashType, byte[] input) => StringUtils.BinaryToHex(CalculateHash(hashType, input));
    public static string CalculateHashHex(HashType hashType, string input) => CalculateHashHex(hashType, Encoding.UTF8.GetBytes(input));
  }
}