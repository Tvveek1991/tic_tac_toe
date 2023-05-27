using System;
using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Network;
using Unity.Netcode;

namespace _Project.Scripts.Features.GameScreen
{
  public class MultiplayerGameViewLogic : GameViewLogic
  {
    private readonly List<BasePlayerViewMono> _spawnPool = new();

    private readonly INetwork _network;

    public MultiplayerGameViewLogic(GameViewMono gameViewMono, ILocaleSystem localeSystem, INetwork network, IGameMetaProvider gameMetaProvider,
      IAudioService audioService, IInputResolver inputResolver)
      : base(gameViewMono, localeSystem, gameMetaProvider, audioService, inputResolver)
    {
      _network = network;
    }

    public override void Construct(Action onBack, Action onCreateNonConnectionPlayer)
    {
      base.Construct(onBack, onCreateNonConnectionPlayer);

      GameViewMono.Construct(ConnectClientObjectServerRpc, ConnectPlayersToNetworkServerRpc, RefreshAfterChangeTurnServerRpc, SetSkinsArrayServerRpc);
    }

    public override void ChangeTurningPlayer(SymbolSign activeSign) =>
      GameViewMono.ChangeTurningPlayerServerRpc();

    [ServerRpc]
    private void ConnectClientObjectServerRpc()
    {
      GameViewMono.SetLocalIpAddress(_network.GetLocalIPAddress());

      OnCreateNonConnectionPlayer?.Invoke();
      _spawnPool.Add(GameMetaProvider.GetPlayerViewMono());
    }

    [ServerRpc]
    private void ConnectPlayersToNetworkServerRpc()
    {
      // Debug.Log("Client Count: " + _network.NetworkManager.ConnectedClients.Count);
      for (var i = 0; i < _network.NetworkManager.ConnectedClients.Count; i++) 
        _spawnPool[i].NetworkObject.SpawnAsPlayerObject((uint) i, true);
      // Debug.Log("SpawnedPlayer + " + i);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSkinsArrayServerRpc()
    {
      SymbolSign symbol = (SymbolSign) GameMetaProvider.GetPlayerViewMono().OwnerClientId + 1;
      GameViewMono.SetSkinIndexServerRpc((int)symbol - 1, GameMetaProvider.GetSkinIndex(symbol));
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshAfterChangeTurnServerRpc(int activePlayer)
    {
      // GameMetaProvider.SetOpponentSkinIndex(GameViewMono.GetSkinIndex(GameMetaProvider.GetPlayerViewMono().OwnerClientId == 0 ? 1 : 0));
      
      GameMetaProvider.GetPlayerViewMono().SetPlayerTurnServerRpc(GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer);
      GameViewMono.TurnText.text = GameMetaProvider.GetPlayerViewMono().OwnerClientId == (ulong) activePlayer ? "Your turn" : "Enemy turn";

      if (GameMetaProvider.GetGame() != Game.Buhurt)
        return;

      SymbolSign symbol = (SymbolSign) activePlayer + 1;
      LoadBuhurtPart(symbol);
      GameViewMono.SetVisibleBuhurtPart(GameMetaProvider.GetPlayerViewMono().OwnerClientId == (uint) activePlayer);
    }
  }
}