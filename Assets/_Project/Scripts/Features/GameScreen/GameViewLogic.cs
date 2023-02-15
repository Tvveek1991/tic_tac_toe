using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.GameMetaProvider;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using _Project.Scripts.Services.Network;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public class GameViewLogic : IGameViewLogic
  {
    private readonly GameViewMono _gameViewMono;

    private readonly INetwork _network;
    private readonly ILocaleSystem _localeSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    private Dictionary<SymbolSign, PlayerData> _playerData;

    private Action _onCreateNonConnectionPlayer;

    private readonly List<PlayerViewMono> _spawnPool = new List<PlayerViewMono>();

    public GameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network, IGameMetaProvider gameMetaProvider)
    {
      _network = network;
      _localeSystem = localeSystem;
      _gameViewMono = gameViewMono;
      _gameMetaProvider = gameMetaProvider;
    }

    public void Construct(IAudioService audioService, Action onBack, Action onCreateNonConnectionPlayer)
    {
      _onCreateNonConnectionPlayer = onCreateNonConnectionPlayer;
      _playerData = _gameMetaProvider.GetPlayerData();

      // Localize(_localeSystem);
      _gameViewMono.gameObject.SetActive(false);
      // AppendClickSound(audioService);

      _gameViewMono.BackButton.onClick.AddListener(() => onBack?.Invoke());

      AppendButtonsAnimation();

      _gameViewMono.Construct(ConnectClientObjectServerRpc, ConnectPlayersToNetworkServerRpc, RefreshAfterChangeTurnServerRpc);
    }

    public void OpenWindow() =>
      AppearAnimation();

    private void AppearAnimation()
    {
      _gameViewMono.gameObject.SetActive(true);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _gameViewMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    private void AppendButtonsAnimation()
    {
      new ButtonAnimation(_gameViewMono.BackButton.transform);
    }

    private void AppendClickSound(IAudioService audioService)
    {
      _gameViewMono.BackButton.onClick.AddListener((() => audioService.Play(AudioId.SoundButton)));
    }


    public void SetPlayerUi(SymbolSign symbol)
    {
      _gameViewMono.SymbolImage.sprite = _playerData[symbol].SymbolData.Icon;
      _gameViewMono.SymbolImage.color = _playerData[symbol].Color;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeTurnServerRpc()
    {
      // _gameViewMono.ChangeTurnServerRpc();
    }

    [ServerRpc]
    private void ConnectClientObjectServerRpc()
    {
      // Debug.Log("Start Spawn Player");
      _onCreateNonConnectionPlayer?.Invoke();
      _spawnPool.Add(_gameMetaProvider.GetPlayerViewMono());
    }

    [ServerRpc]
    private void ConnectPlayersToNetworkServerRpc()
    {
      // Debug.Log("Client Count: " + _network.NetworkManager.ConnectedClients.Count);
      for (var i = 0; i < _network.NetworkManager.ConnectedClients.Count; i++)
      {
        _spawnPool[i].NetworkObject.SpawnAsPlayerObject((uint) i, true);
        // Debug.Log("SpawnedPlayer + " + i);
      }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RefreshAfterChangeTurnServerRpc(int activePlayer)
    {
      _gameMetaProvider.GetPlayerViewMono().SetPlayerTurnServerRpc(_gameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer);
      _gameViewMono.TurnText.text = _gameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer ? "Your turn" : "Enemy turn";
    }
  }
}