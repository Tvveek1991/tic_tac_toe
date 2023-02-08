using UnityEngine;

namespace _Project.Scripts.Services.Network.Mono
{
  public class Singleton : MonoBehaviour
  {
    public static Singleton Instance;

    protected void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
      }
    }
  }
}