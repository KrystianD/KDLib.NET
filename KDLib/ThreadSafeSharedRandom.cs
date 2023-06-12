using System;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class ThreadSafeSharedRandom
  {
#if NET6_0
    public static int RandInt() => Random.Shared.Next();
    public static int RandInt(int maxValue) => Random.Shared.Next(maxValue);
    public static int RandInt(int minValue, int maxValue) => Random.Shared.Next(minValue, maxValue);
#else
    private static readonly Random Shared = new Random();

    public static int RandInt()
    {
      lock (Shared) {
        return Shared.Next();
      }
    }

    public static int RandInt(int maxValue)
    {
      lock (Shared) {
        return Shared.Next(maxValue);
      }
    }

    public static int RandInt(int minValue, int maxValue)
    {
      lock (Shared) {
        return Shared.Next(minValue, maxValue);
      }
    }
#endif
  }
}