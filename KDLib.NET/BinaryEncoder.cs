using System;
using JetBrains.Annotations;

namespace KDLib
{
  public enum BinaryEncoding
  {
    Base64,
    Base62,
  }

  [PublicAPI]
  public static class BinaryEncoder
  {
    public static string Encode(byte[] data, BinaryEncoding encoding)
    {
      switch (encoding) {
        case BinaryEncoding.Base64: return Convert.ToBase64String(data);
        case BinaryEncoding.Base62: return Base62Converter.Encode(data);
        default: throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
      }
    }

    public static byte[] Decode(string data, BinaryEncoding encoding)
    {
      switch (encoding) {
        case BinaryEncoding.Base64: return Convert.FromBase64String(data);
        case BinaryEncoding.Base62: return Base62Converter.Decode(data);
        default: throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
      }
    }
  }
}