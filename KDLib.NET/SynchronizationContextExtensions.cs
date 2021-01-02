using System;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static class SynchronizationContextExtensions
  {
    public static void Post(this SynchronizationContext ctx, Action d) => ctx.Post(_ => { d(); }, null);
  }
}