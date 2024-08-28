using System;
using System.Threading;
using System.Threading.Tasks;

namespace KDLib
{
  public static partial class AsyncUtils
  {
    #region SelectAsync - 2 params
    public static async Task Select<T1, T2>(Func<CancellationToken, Task<T1>> task1,
                                            Action<T1> a1,
                                            Func<CancellationToken, Task<T2>> task2,
                                            Action<T2> a2,
                                            CancellationToken token = default)
      => await Select()
               .Case(task1, a1)
               .Case(task2, a2)
               .WaitAsync(token);

    public static async Task Select<T1>(Func<CancellationToken, Task<T1>> task1,
                                        Action<T1> a1,
                                        Func<CancellationToken, Task> task2,
                                        Action a2,
                                        CancellationToken token = default)
      => await Select()
               .Case(task1, a1)
               .Case(task2, a2)
               .WaitAsync(token);

    public static async Task Select<T2>(Func<CancellationToken, Task> task1,
                                        Action a1,
                                        Func<CancellationToken, Task<T2>> task2,
                                        Action<T2> a2,
                                        CancellationToken token = default)
      => await Select()
               .Case(task1, a1)
               .Case(task2, a2)
               .WaitAsync(token);
    #endregion
  }
}