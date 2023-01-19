using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Games
{
  public abstract class BaseFieldLogic
  {
    protected readonly GameFieldView GameFieldView;
    
    protected IAudioService AudioService;

    protected BaseFieldLogic(GameFieldView gameFieldView, IAudioService audioService)
    {
      GameFieldView = gameFieldView;
      AudioService = audioService;
    }
  }
}