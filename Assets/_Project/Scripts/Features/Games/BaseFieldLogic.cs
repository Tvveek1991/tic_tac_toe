using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Games
{
  public abstract class BaseFieldLogic
  {
    protected readonly GameField GameField;
    
    protected readonly IAudioService AudioService;
    protected readonly IGameMetaProvider GameMetaProvider;

    protected BaseFieldLogic(GameField gameField, IAudioService audioService, IGameMetaProvider gameMetaProvider)
    {
      GameField = gameField;
      AudioService = audioService;
      GameMetaProvider = gameMetaProvider;
    }
  }
}