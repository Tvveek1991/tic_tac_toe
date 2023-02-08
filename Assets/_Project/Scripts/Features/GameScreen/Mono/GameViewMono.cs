using System;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameViewMono : NetworkBehaviour
  {
    [Space(20)] [SerializeField] private Button backButton;

    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private TextMeshProUGUI loadProgress;

    [Header("Player UI")] [SerializeField] private CanvasGroup TurnStatus;
    [SerializeField] private Image symbolImage;

    private readonly NetworkVariable<int> _netPlayersCount = new NetworkVariable<int>();

    private Action _onClientConnect;
    private Action _onLoadComplete;

    public Button BackButton => backButton;
    public CanvasGroup LoadScreen => loadScreen;
    public TextMeshProUGUI LoadProgress => loadProgress;
    public Image SymbolImage => symbolImage;

    public void Construct(Action onClientConnect, Action onLoadComplete)
    {
      _onLoadComplete = onLoadComplete;
      _onClientConnect = onClientConnect;
    }

    public override void OnNetworkSpawn()
    {
      _netPlayersCount.OnValueChanged += OnValueChanged;
      CheckClientsCountServerRpc();
    }

    public override void OnDestroy() =>
      _netPlayersCount.OnValueChanged -= OnValueChanged;

    private void OnValueChanged(int previous, int playersCount)
    {
      LoadProgress.text = playersCount + "/2 Players";

      if (IsHost)
        _onClientConnect?.Invoke();

      if (playersCount < 2) return;

      if (IsHost)
        _onLoadComplete?.Invoke();

      LoadScreen.DOFade(0, .5f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckClientsCountServerRpc() =>
      _netPlayersCount.Value = NetworkManager.ConnectedClients.Count;
  }
}