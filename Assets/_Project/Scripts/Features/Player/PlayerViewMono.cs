using System;
using System.Collections.Generic;
using _Project.Scripts.Features.Games;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;
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

    public Image SymbolImage;

    [Space(20)] public CanvasGroup PlayerColor;
    public CanvasGroup PlayerStatus;

    public override void OnDestroy()
    {
      _netSymbolSign.OnValueChanged -= OnValueChanged;
    }

    public void Construct(IGameMetaProvider playerData)
    {
      _playerData = playerData.GetPlayerData(); //ToDO Основной камень предкновения здесь (Не удалось привязать Player клиента к основному дереву)
      Debug.Log(_playerData == null);
    }

    private void OnValueChanged(SymbolSign previous, SymbolSign next) => 
      ChangeIcon(next);

    public override void OnNetworkSpawn()
    {
      OnCreate();
    }

    private void OnCreate()
    {
      transform.position = _spawnPosition;
      
      _netSymbolSign.OnValueChanged += OnValueChanged;

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

        ChangeIcon(_netSymbolSign.Value);

        enabled = false;
      }
    }

    private void ChangeIcon(SymbolSign symbol)
    {
      /*SymbolImage.sprite = _playerData[symbol].SymbolData.Icon;
      SymbolImage.color = _playerData[symbol].Color;*/
    }

    private void Update()
    {
      //ToDo Переделать связку с объектами
      if (!Input.GetMouseButtonDown(0) || !IsOwner)
        return;

      if (IsOwner)
        Debug.Log(name);

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
      (SymbolSign) (_index + 1);
  }
}