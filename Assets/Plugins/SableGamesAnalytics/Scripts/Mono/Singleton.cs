using UnityEngine;

namespace SableGamesAnalytics.Scripts.Mono
{
  public class Singleton<T> : MonoBehaviour
  {
    private static Singleton<T> _instance;

    private void Awake()
    {
      if (_instance == null)
      {
        _instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
        Destroy(gameObject);
    }
  }
}