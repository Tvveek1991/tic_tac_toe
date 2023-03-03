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

    [SerializeField] private TextMeshProUGUI myLocalIp;

    [Header("Player UI")] [SerializeField] private CanvasGroup turnCanvasGroup;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Image symbolImage;

    private readonly NetworkVariable<int> _netPlayersCount = new NetworkVariable<int>();
    private readonly NetworkVariable<int> _netActivePlayer = new NetworkVariable<int>();

    private Action<int> _onChosenFirstTurningPlayer;
    private Action _onClientConnect;
    private Action _onConnectedPlayersToNetwork;

    public Button BackButton => backButton;
    public TextMeshProUGUI TurnText => turnText;
    public Image SymbolImage => symbolImage;

    public void Construct(Action onClientConnect, Action onConnectedPlayersToNetwork, Action<int> refreshAfterChangeTurn)
    {
      _onClientConnect = onClientConnect;
      _onChosenFirstTurningPlayer = refreshAfterChangeTurn;
      _onConnectedPlayersToNetwork = onConnectedPlayersToNetwork;
    }

    public void HideLoadScreen()
    {
      loadScreen.DOFade(0, .5f);
    }
    
    #region Multiplayer

    public override void OnNetworkSpawn()
    {
      _netPlayersCount.OnValueChanged += OnCountChanged;
      _netActivePlayer.OnValueChanged += OnActivePlayerChanged;
      CheckClientsCountServerRpc();
    }

    public override void OnDestroy()
    {
      _netPlayersCount.OnValueChanged -= OnCountChanged;
      _netActivePlayer.OnValueChanged -= OnActivePlayerChanged;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeTurningPlayerServerRpc() =>
      _netActivePlayer.Value = (_netActivePlayer.Value + 1) % NetworkManager.ConnectedClients.Count;

    public void SetLocalIpAddress(string getLocalIPAddress) =>
      myLocalIp.text = getLocalIPAddress;

    [ServerRpc]
    private void SetFirstTurningPlayerServerRpc()
    {
      _netActivePlayer.Value = Random.Range(0, NetworkManager.ConnectedClients.Count);
      Debug.Log("First Player is " + _netActivePlayer.Value);
    }

    private void OnActivePlayerChanged(int previous, int newValue) =>
      _onChosenFirstTurningPlayer?.Invoke(newValue);

    private void OnCountChanged(int previous, int playersCount)
    {
      loadProgress.text = playersCount + "/2 Players";

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

      HideLoadScreen();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckClientsCountServerRpc() =>
      _netPlayersCount.Value = NetworkManager.ConnectedClients.Count;

    #endregion
  }
}