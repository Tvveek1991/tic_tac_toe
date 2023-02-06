using System.Collections.Generic;
using _Project.Scripts.Features.Games;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services.GameMetaProvider;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Player
{
  public class PlayerViewMono : NetworkBehaviour
  {
    private readonly Vector3 _spawnPosition = new Vector3(0, 1, -10);

    private readonly NetworkVariable<SymbolSign> _netSymbolSign = new NetworkVariable<SymbolSign>();

    private int _index;

    private Dictionary<SymbolSign, PlayerData> _playerData;

    public List<Sprite> Sprites = new List<Sprite>();

    public Image SymbolImage;

    [Space(20)] public CanvasGroup PlayerColor;
    public CanvasGroup PlayerStatus;

    public override void OnDestroy()
    {
      _netSymbolSign.OnValueChanged -= OnValueChanged;
    }

    public void Construct(Dictionary<SymbolSign, PlayerData> playerData)
    {
      _playerData = playerData;
    }

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      // SymbolImage.sprite = _playerData[next].SymbolData.Icon;
       // SymbolImage.color = _playerData[next].Color;
    }

    public override void OnNetworkSpawn()
    {
      OnCreate();
    }

    private void OnCreate()
    {
      transform.position = _spawnPosition;

      _netSymbolSign.OnValueChanged += OnValueChanged;
        
      Debug.Log("newPlayer_: " + OwnerClientId);
      name = "newPlayer_" + OwnerClientId;

      if (IsOwner)
      {
        _index = (int) OwnerClientId;
        CommitNetworkColorServerRpc(GetNewColor());
      }
      else
      {
        PlayerColor.alpha = 0;
        PlayerStatus.alpha = 0;

        // SymbolImage.sprite = _playerData[_netSymbolSign.Value].SymbolData.Icon;
        // SymbolImage.color = _playerData[_netSymbolSign.Value].Color;

        enabled = false;
      }
    }

    private void Update()
    {
      //ToDo Переделать связку с объектами
      if (!Input.GetMouseButtonDown(0) || !IsOwner)
        return;

      if (IsHost)
        Debug.Log(NetworkManager.ConnectedClients.Count);

      CheckCell();
    }

    private void CheckCell()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit))
      {
        GameObject hitObject = hit.transform.gameObject;
        hitObject.GetComponent<SymbolCell>().SetSymbolServerRpc(_netSymbolSign.Value);
      }
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(SymbolSign symbol) =>
      _netSymbolSign.Value = symbol;

    private SymbolSign GetNewColor() =>
      (SymbolSign)(_index + 1);
  }
}