using System;
using System.Threading.Tasks;

namespace _Tools
{
  public static class AsyncEvents
  {
    public static Task TaskFromEvent(
      Action<Action> addHandler,
      Action<Action> removeHandler)
    {
      var task = new Task(() => { });

      addHandler(Handle);

      void Handle()
      {
        removeHandler(Handle);
        task.Start();
      }

      return task;
    }

    public static Task<T> TaskFromEvent<T>(
      Action<Action<T>> addHandler,
      Action<Action<T>> removeHandler)
    {
      var source = new TaskCompletionSource<T>();

      addHandler(Handle);

      void Handle(T arg)
      {
        removeHandler(Handle);
        source.SetResult(arg);
      }

      return source.Task;
    }
  }
}
