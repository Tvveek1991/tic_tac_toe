using System;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameViewMono : NetworkBehaviour
  {
    [Space(20)] [SerializeField] private Button backButton;

    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private TextMeshProUGUI loadProgress;

    [Header("Player UI")] [SerializeField] private CanvasGroup turnCanvasGroup;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Image symbolImage;

    private readonly NetworkVariable<int> _netPlayersCount = new NetworkVariable<int>();
    private readonly NetworkVariable<int> _netActivePlayer = new NetworkVariable<int>();

    private Action<int> _onChosenFirstTurningPlayer;
    private Action _onClientConnect;
    private Action _onConnectedPlayersToNetwork;

    public Button BackButton => backButton;
    public CanvasGroup LoadScreen => loadScreen;
    public TextMeshProUGUI LoadProgress => loadProgress;
    public TextMeshProUGUI TurnText => turnText;
    public Image SymbolImage => symbolImage;

    public void Construct(Action onClientConnect, Action onConnectedPlayersToNetwork, Action<int> refreshAfterChangeTurn)
    {
      _onClientConnect = onClientConnect;
      _onChosenFirstTurningPlayer = refreshAfterChangeTurn;
      _onConnectedPlayersToNetwork = onConnectedPlayersToNetwork;
    }

    public override void OnNetworkSpawn()
    {
      _netPlayersCount.OnValueChanged += OnValueChanged;
      _netActivePlayer.OnValueChanged += OnValueActivePlayerChanged;
      CheckClientsCountServerRpc();
    }

    public override void OnDestroy()
    {
      _netPlayersCount.OnValueChanged -= OnValueChanged;
      _netActivePlayer.OnValueChanged -= OnValueActivePlayerChanged;
    }

    [ServerRpc]
    public void ChangeTurnServerRpc() => 
      _netActivePlayer.Value = ++_netActivePlayer.Value % NetworkManager.ConnectedClients.Count;
    
    private void OnValueActivePlayerChanged(int previous, int newValue)
    {
      Debug.Log("it is changed Oo + " + newValue );
    }

    private void OnValueChanged(int previous, int playersCount)
    {
      LoadProgress.text = playersCount + "/2 Players";

      if (IsHost)
        _onClientConnect?.Invoke();

      if (playersCount < 2)
        return;

      if (IsHost)
      {
        _onConnectedPlayersToNetwork?.Invoke();
        SetFirstTurningPlayerServerRpc();
      }

      _onChosenFirstTurningPlayer?.Invoke(_netActivePlayer.Value);
      
      LoadScreen.DOFade(0, .5f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckClientsCountServerRpc() =>
      _netPlayersCount.Value = NetworkManager.ConnectedClients.Count;

    [ServerRpc]
    private void SetFirstTurningPlayerServerRpc()
    {
      _netActivePlayer.Value = Random.Range(0, NetworkManager.ConnectedClients.Count);
      Debug.Log("First Player is " + _netActivePlayer.Value);
    }
  }
}