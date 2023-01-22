using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Games
{
  public abstract class BaseFieldLogic
  {
    protected readonly GameField GameField;
    
    protected IAudioService AudioService;

    protected BaseFieldLogic(GameField gameField, IAudioService audioService)
    {
      GameField = gameField;
      AudioService = audioService;
    }
  }
}