using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class BinaryUtils
  {
    public static int StructSize<T>() => Marshal.SizeOf(typeof(T));
    public static int StructSize(object obj) => Marshal.SizeOf(obj.GetType());

    public static T StructFromBytes<T>(byte[] bytes)
    {
      GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      T obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
      handle.Free();

      return obj;
    }

    public static byte[] StructToBytes<T>(T obj)
    {
      byte[] bytes = new byte[StructSize<T>()];

      GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
      Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), true);
      handle.Free();

      return bytes;
    }

    public static short SwapBytes(short x) => (short)SwapBytes((ushort)x);

    public static ushort SwapBytes(ushort x)
    {
      return (ushort)(((x & 0xff00) >> 8) |
                      ((x & 0x00ff) << 8));
    }

    public static int SwapBytes(int x) => (int)SwapBytes((uint)x);

    public static uint SwapBytes(uint x)
    {
      return ((x & 0xff000000) >> 24) |
             ((x & 0x00ff0000) >> 8) |
             ((x & 0x0000ff00) << 8) |
             ((x & 0x000000ff) << 24);
    }

    public static long SwapBytes(long x) => (long)SwapBytes((ulong)x);

    public static ulong SwapBytes(ulong x)
    {
      x = ((x & 0xffffffff00000000) >> 32) |
          ((x & 0x00000000ffffffff) << 32);
      x = ((x & 0xffff0000ffff0000) >> 16) |
          ((x & 0x0000ffff0000ffff) << 16);
      x = ((x & 0xff00ff00ff00ff00) >> 8) |
          ((x & 0x00ff00ff00ff00ff) << 8);
      return x;
    }

#if NETSTANDARD2_0
    public static float SwapBytes(float x)
    {
      var bytes = BitConverter.GetBytes(x);
      Array.Reverse(bytes);
      return BitConverter.ToSingle(bytes, 0);
    }
#else
    public static float SwapBytes(float x) => BitConverter.Int32BitsToSingle(SwapBytes(BitConverter.SingleToInt32Bits(x)));
#endif
    public static double SwapBytes(double x) => BitConverter.Int64BitsToDouble(SwapBytes(BitConverter.DoubleToInt64Bits(x)));

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public static void SwapStructureEndianness<T>(ref T obj)
    {
      foreach (var fi in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        switch (fi.GetValue(obj)) {
          case short v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case ushort v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case int v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case uint v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case long v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case ulong v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case float v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
          case double v:
            fi.SetValueDirect(__makeref(obj), SwapBytes(v));
            break;
        }
      }
    }
  }
}