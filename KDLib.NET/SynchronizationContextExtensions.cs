using System;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static class SynchronizationContextExtensions
  {
    public static void Post(this SynchronizationContext ctx, SendOrPostCallback d) => ctx.Post(d, null);
    public static void Post(this SynchronizationContext ctx, Action d) => ctx.Post(_ => { d(); }, null);
    public static void Post(this SynchronizationContext ctx, Func<Task> d) => ctx.Post(_ => { d(); }, null);
  }
}