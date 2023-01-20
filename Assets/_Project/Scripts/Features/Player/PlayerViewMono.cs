using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Player
{
  public class PlayerViewMono : NetworkBehaviour
  {
    private readonly NetworkVariable<Color> _netColor = new NetworkVariable<Color>();
    private readonly NetworkVariable<bool> _netMessage = new NetworkVariable<bool>();
    
    private readonly List<Color> _colors = new List<Color> {Color.red, Color.blue, Color.green, Color.yellow};
    
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
      }
    }

    [ServerRpc]
    public void CommitNetworkMessageServerRpc(string message) => 
      Message.text = message;

    [ServerRpc]
    private void CommitNetworkColorServerRpc(Color color)
    {
      print(_colors.Count);
      _netColor.Value = color;
      _colors.Remove(color);
    }

    private Color GetNewColor()
    {
      // return _colors[Random.Range(0, _colors.Count)];
      return _colors[_index++ % _colors.Count];
    }
  }
}