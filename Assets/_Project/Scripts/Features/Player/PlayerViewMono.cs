using System.Collections.Generic;
using _Project.Scripts.Features.Games;
using _Project.Scripts.Features.GameScreen.Serialize;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Player
{
  public class PlayerViewMono : NetworkBehaviour
  {
    private readonly Vector3 _spawnPosition = new Vector3(0, 1, -10);

    private readonly NetworkVariable<SymbolSign> _netSymbolSign = new NetworkVariable<SymbolSign>();
    private readonly NetworkVariable<bool> _netMessage = new NetworkVariable<bool>();

    private readonly List<Color> _colors = new List<Color> {Color.green, Color.red, Color.blue};
    
    private int _index;

    public List<Sprite> Sprites = new List<Sprite>();
    
    public Image SymbolImage;
    public TextMeshProUGUI Message;

    [Space(20)] public CanvasGroup PlayerColor;
    public CanvasGroup PlayerStatus;

    public override void OnDestroy()
    {
      _netSymbolSign.OnValueChanged -= OnValueChanged;
      _netMessage.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(SymbolSign previous, SymbolSign next)
    {
      SymbolImage.sprite = Sprites[(int) next];
      SymbolImage.color = _colors[(int)next];
    }

    private void OnValueChanged(bool previous, bool next) =>
      Message.text = next ? "Your turn" : "Enemy turn";

    public override void OnNetworkSpawn()
    {
      Debug.Log("ID: " + OwnerClientId);
      transform.position = _spawnPosition;
      
      _netSymbolSign.OnValueChanged += OnValueChanged;
      _netMessage.OnValueChanged += OnValueChanged;

      OnCreate();
    }

    private void OnCreate()
    {
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
        
        SymbolImage.sprite = Sprites[(int)_netSymbolSign.Value];
        SymbolImage.color = _colors[(int)_netSymbolSign.Value];

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