using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace KDLib
{
  public enum BinaryEncoding
  {
    Base64,
    Base62,
    Hex,
  }

  [PublicAPI]
  public static class BinaryEncoder
  {
    public static string Encode(byte[] data, BinaryEncoding encoding)
    {
      switch (encoding) {
        case BinaryEncoding.Base64: return Convert.ToBase64String(data);
        case BinaryEncoding.Base62: return Base62Converter.Encode(data);
        case BinaryEncoding.Hex:
          return data.Select(x => x.ToString("x2"))
                     .JoinString();
        default: throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
      }
    }

    public static string Encode(IEnumerable<byte> data, BinaryEncoding encoding) => Encode(data.ToArray(), encoding);

    public static byte[] Decode(string data, BinaryEncoding encoding)
    {
      switch (encoding) {
        case BinaryEncoding.Base64: return Convert.FromBase64String(data);
        case BinaryEncoding.Base62: return Base62Converter.Decode(data);
        case BinaryEncoding.Hex:
          return Enumerable.Range(0, data.Length)
                           .Where(x => x % 2 == 0)
                           .Select(x => Convert.ToByte(data.Substring(x, 2), 16))
                           .ToArray();
        default: throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
      }
    }
  }
}