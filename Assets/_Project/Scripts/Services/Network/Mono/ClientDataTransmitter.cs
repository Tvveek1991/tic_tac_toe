using System.Collections;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Network.Mono
{
  public class ClientDataTransmitter : MonoBehaviour
  {
    public static ClientDataTransmitter Instance;

    private IGameFactory _gameFactory;
    private IInputResolver _inputResolver;
    private IGameMetaProvider _gameMetaProvider;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
        Destroy(gameObject);
    }

    public void Construct(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      _gameFactory = gameFactory;
      _inputResolver = inputResolver;
      _gameMetaProvider = gameMetaProvider;

      // StartCoroutine(GetExternalIP());
    }

    public IGameFactory GetGameFactory() =>
      _gameFactory;

    public IGameMetaProvider GetGameMetaProvider() =>
      _gameMetaProvider;

    public IInputResolver GetInputResolver() =>
      _inputResolver;

    /*private IEnumerator GetExternalIP()
    {
      string url = "https://api.ipify.org";
      using var www = new WWW(url);
      yield return www;

      if (!string.IsNullOrEmpty(www.error))
        Debug.LogErrorFormat("Failed to get external IP: {0}", www.error);
      else
      {
        string externalIP = www.text;
        Debug.Log("External IP address: " + externalIP);
      }
    }*/
  }
}