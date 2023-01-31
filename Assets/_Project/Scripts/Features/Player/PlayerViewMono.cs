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

    private readonly NetworkVariable<Color> _netColor = new NetworkVariable<Color>();
    private readonly NetworkVariable<bool> _netMessage = new NetworkVariable<bool>();

    private readonly List<Color> _colors = new List<Color> {Color.red, Color.blue, Color.green, Color.yellow};
    private SymbolSign _symbolSign;
    
    private int _index;

    public Image SymbolColor;
    public TextMeshProUGUI Message;

    [Space(20)] public CanvasGroup PlayerColor;
    public CanvasGroup PlayerStatus;

    public override void OnDestroy()
    {
      _netColor.OnValueChanged -= OnValueChanged;
      _netMessage.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(Color previous, Color next) =>
      SymbolColor.color = next;

    private void OnValueChanged(bool previous, bool next) =>
      Message.text = next ? "Your turn" : "Enemy turn";

    public override void OnNetworkSpawn()
    {
      transform.position = _spawnPosition;
      
      _netColor.OnValueChanged += OnValueChanged;
      _netMessage.OnValueChanged += OnValueChanged;

      OnCreate();
    }

    private void OnCreate()
    {
      if (IsOwner)
      {
        _index = (int) OwnerClientId;
        CommitNetworkColorServerRpc(GetNewColor());
      }
      else
      {
        PlayerColor.alpha = 0;
        PlayerStatus.alpha = 0;

        SymbolColor.color = _netColor.Value;

        enabled = false;
      }
    }

    private void Update()
    {
      //ToDo Переделать связку с объектами
      if (!Input.GetMouseButtonDown(0) || !IsOwner)
        return;
      
      CheckCell();
    }

    private void CheckCell()
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast(ray, out hit))
      {
        GameObject hitObject = hit.transform.gameObject;
        Debug.Log(hitObject.name);

        hitObject.GetComponent<SymbolCell>().SetSymbolServerRpc(_symbolSign);
      }
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(Color color)
    {
      _netColor.Value = color;
      _colors.Remove(color);
    }

    private Color GetNewColor()
    {
      _symbolSign = (SymbolSign)(_index + 1);
      return _colors[_index++ % _colors.Count];
    }
  }
}