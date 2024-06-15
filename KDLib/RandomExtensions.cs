using System;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class RandomExtensions
  {
#if !NET6_0_OR_GREATER
    public static float NextSingle(this Random random) => (float)random.NextDouble();
#endif

    public static float NextSingle(this Random random, float max) => random.NextSingle() * max;
    public static float NextSingle(this Random random, float min, float max) => random.NextSingle() * (max - min) + min;

    public static double NextDouble(this Random random, double max) => random.NextDouble() * max;
    public static double NextDouble(this Random random, double min, double max) => random.NextDouble() * (max - min) + min;
  }
}