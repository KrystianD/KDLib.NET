using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace KDLib
{
  public enum hashType
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
    public static byte[] CalculateHash(hashType hashType, byte[] input)
    {
      using HashAlgorithm hash = hashType switch {
          hashType.MD5 => MD5.Create(),
          hashType.SHA1 => SHA1.Create(),
          hashType.SHA256 => SHA256.Create(),
          hashType.SHA384 => SHA384.Create(),
          hashType.SHA512 => SHA512.Create(),
          _ => throw new ArgumentException("Invalid hash type"),
      };
      return hash!.ComputeHash(input);
    }

    public static byte[] CalculateHash(hashType hashType, string input) => CalculateHash(hashType, Encoding.UTF8.GetBytes(input));

    public static string CalculateHashHex(hashType hashType, byte[] input) => StringUtils.BinaryToHex(CalculateHash(hashType, input));
    public static string CalculateHashHex(hashType hashType, string input) => CalculateHashHex(hashType, Encoding.UTF8.GetBytes(input));
  }
}