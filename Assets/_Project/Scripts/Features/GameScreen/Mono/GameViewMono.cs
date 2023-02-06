using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Network;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameViewMono : NetworkBehaviour
  {
    [SerializeField] private List<PlayerViewMono> _playersViewMono;
    
    [Space(20)]
    [SerializeField] private Button backButton;

    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private TextMeshProUGUI loadProgress;

    private readonly NetworkVariable<int> _netPlayersCount = new NetworkVariable<int>();

    public Button BackButton => backButton;
    public CanvasGroup LoadScreen => loadScreen;
    public TextMeshProUGUI LoadProgress => loadProgress;

    private INetwork _network;
    private IGameMetaProvider _gameMetaProvider;

    private Action _onLoadClient;

    public void Construct(INetwork network, IGameMetaProvider gameMetaProvider, Action onLoadClient)
    {
      _network = network;
      _onLoadClient = onLoadClient;
      _gameMetaProvider = gameMetaProvider;
      
      _playersViewMono.ForEach(item => item.Construct(gameMetaProvider));
    }

    public override void OnDestroy() =>
      _netPlayersCount.OnValueChanged -= OnValueChanged;

    public override void OnNetworkSpawn()
    {
      _netPlayersCount.OnValueChanged += OnValueChanged;

      CheckPlayersServerRpc();
    }

    private void OnValueChanged(int previous, int next)
    {
      LoadProgress.text = next + "/2 Players";

      if (IsOwner)
      {
        _onLoadClient?.Invoke();
      }

      if (next == 2)
      {
        LoadScreen.DOFade(0, .5f);

        if (IsHost)
        {
          _network.ConnectPlayersToNetwork();
        }
      }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckPlayersServerRpc()
    {
      Debug.Log("I am " + OwnerClientId);
      Debug.Log(NetworkManager.ConnectedClients.Count);

      _netPlayersCount.Value = _network.NetworkManager.ConnectedClients.Count;
    }
  }
}