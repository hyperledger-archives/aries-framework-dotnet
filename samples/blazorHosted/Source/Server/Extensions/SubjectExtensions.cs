namespace System.Reactive.Subjects
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  public static class SubjectExtensions
  {
    public static Task<T> AsAwaitable<T>(this IObservable<T> aSubject, TimeSpan aTimeout) => 
      aSubject.AsAwaitable((int)aTimeout.TotalMilliseconds);

    public static async Task<T> AsAwaitable<T>(this IObservable<T> aSubject, int aMilliseconds)
    {
      var semaphoreSlim = new SemaphoreSlim(0, 1);

      T result = default;
      using
      (
        IDisposable subscription = aSubject
        .Subscribe
        (
          aValue =>
          {
            result = aValue;
            semaphoreSlim.Release();
          }
        )
      )
      {
        bool success = await semaphoreSlim.WaitAsync(aMilliseconds);
        if (!success) throw new TimeoutException();
      }
      return result;
    }
  }
}
