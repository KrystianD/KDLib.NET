using System;
using System.Threading;
using JetBrains.Annotations;

namespace KDLib
{
  [PublicAPI]
  public static class SynchronizationContextExtensions
  {
    public static void Post(this SynchronizationContext ctx, Action d) => ctx.Post(_ => { d(); }, null);
  }
}