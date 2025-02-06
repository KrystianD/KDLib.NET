using System.Threading;
using System.Threading.Tasks;

namespace KDLib.RateLimiters;

public interface IAsyncRateLimiter
{
  Task AcquireAsync(CancellationToken token = default);
}