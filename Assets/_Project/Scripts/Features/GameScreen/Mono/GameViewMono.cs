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
    [SerializeField] private Button backButton;

    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private TextMeshProUGUI loadProgress;

    private readonly NetworkVariable<int> _netPlayersCount = new NetworkVariable<int>();

    public Button BackButton => backButton;
    public CanvasGroup LoadScreen => loadScreen;
    public TextMeshProUGUI LoadProgress => loadProgress;

    private INetwork _network;
    
    public void Construct(INetwork network)
    {
      _network = network;
    }
    
    public override void OnDestroy()
    {
      _netPlayersCount.OnValueChanged -= OnValueChanged;
    }

    public override void OnNetworkSpawn()
    {
      _netPlayersCount.OnValueChanged += OnValueChanged;

      CheckPlayersServerRpc();
    }

    private void OnValueChanged(int previous, int next)
    {
      LoadProgress.text = next + "/2 Players";

      if (next == 2)
      {
        LoadScreen.DOFade(0, .5f);

        if (IsHost)
          _network.SpawnPlayersObject();
      }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckPlayersServerRpc()
    {
      Debug.Log("I am " + OwnerClientId);
      Debug.Log(NetworkManager.ConnectedClients.Count);

      _netPlayersCount.Value = NetworkManager.ConnectedClients.Count;
    }
  }
}