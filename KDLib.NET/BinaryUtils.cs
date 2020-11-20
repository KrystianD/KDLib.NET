using System.Runtime.InteropServices;

namespace KDLib
{
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
  }
}