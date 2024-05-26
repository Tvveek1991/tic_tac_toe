using System;
using System.Collections.Generic;
using SableGamesAnalytics.Scripts.Mono;
using UnityEngine;

namespace SableGamesAds.Scripts.Common
{
  /// <summary>
  /// Lets you queue calls to Unity API from another thread.<br/>
  /// See <a href="https://stackoverflow.com/a/41333540/5791444">Use Unity API from another Thread</a>
  /// </summary>
  public class UnityThread : Singleton<UnityThread>
  {
    private static readonly Queue<Action> Actions = new Queue<Action>();

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
      var gameObject = new GameObject("UnityThreadQueue");
      gameObject.AddComponent<UnityThread>();
    }

    public static void Enqueue(Action action)
    {
      lock (Actions)
      {
        Actions.Enqueue(action);
      }
    }

    private void Update()
    {
      lock (Actions)
      {
        while (Actions.Count > 0)
        {
          var action = Actions.Dequeue();
          action?.Invoke();
        }
      }
    }
  }
}