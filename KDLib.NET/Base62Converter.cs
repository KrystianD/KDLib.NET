// based on https://github.com/ghost1face/base62/blob/master/Base62/Base62Converter.cs

using System;
using System.Collections.Generic;
using System.Text;

namespace KDLib
{
  public static class Base62Converter
  {
    private const string CharacterSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(byte[] value)
    {
      var converted = BaseConvert(value, 256, 62);

      var builder = new StringBuilder();
      foreach (var b in converted)
        builder.Append(CharacterSet[b]);
      return builder.ToString();
    }

    public static byte[] Decode(string value)
    {
      var arr = new byte[value.Length];
      for (var i = 0; i < arr.Length; i++)
        arr[i] = (byte)CharacterSet.IndexOf(value[i]);

      return BaseConvert(arr, 62, 256);
    }

    private static byte[] BaseConvert(byte[] source, int sourceBase, int targetBase)
    {
      var result = new List<int>();
      int count = 0;

      while ((count = source.Length) > 0) {
        var quotient = new List<byte>();
        int remainder = 0;

        for (var i = 0; i != count; i++) {
          int accumulator = source[i] + remainder * sourceBase;
          byte digit = Convert.ToByte((accumulator - accumulator % targetBase) / targetBase);
          remainder = accumulator % targetBase;

          if (quotient.Count > 0 || digit != 0)
            quotient.Add(digit);
        }

        result.Insert(0, remainder);
        source = quotient.ToArray();
      }

      var output = new byte[result.Count];
      for (int i = 0; i < result.Count; i++)
        output[i] = (byte)result[i];

      return output;
    }
  }
}