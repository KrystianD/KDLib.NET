using System;
using System.Collections.Generic;
using System.Text;

namespace KDLib
{
  public static class StringUtils
  {
    private static readonly Random _stringRandom = new Random();

    public static string GenerateRandomString(
        int length,
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
      var stringChars = new char[length];

      lock (_stringRandom) {
        for (var i = 0; i < stringChars.Length; i++)
          stringChars[i] = chars[_stringRandom.Next(chars.Length)];
      }

      return new string(stringChars);
    }

    public static string BinaryToHex(IEnumerable<byte> data)
    {
      StringBuilder sb = new StringBuilder();
      foreach (var t in data)
        sb.Append(t.ToString("x2"));
      return sb.ToString();
    }
  }
}