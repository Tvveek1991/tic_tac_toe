using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services.GameMetaProvider;

namespace _Project.Scripts.Features.Games.Classic
{
  public interface IClassicFieldLogic
  {
    void Construct(Dictionary<SymbolSign, PlayerData> playerData);
  }
}