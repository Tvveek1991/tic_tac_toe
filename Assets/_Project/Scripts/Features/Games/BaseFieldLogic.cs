using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Games
{
  public abstract class BaseFieldLogic
  {
    public GameFieldView GameField { get; set; }
    
    protected IAudioService AudioService;

    protected BaseFieldLogic(IAudioService audioService)
    {
      AudioService = audioService;
    }
  }
}