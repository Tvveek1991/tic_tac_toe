using _Project.Scripts.Features.EndGameScreen;
using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.Player;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    WarningView WarningView { get; }
    BasePlayerViewMono PlayerViewMono { get; }
    void CreateScreen();
    void CreateField();
    void CreateWarningView();
    void CreateEndGame();
    void SpawnPlayerObject();
    void ClearScreen();
    IGameFieldLogic GameFieldLogic { get; }
    IGameViewLogic GameViewLogic { get; }
    IEndGameLogic EndGameLogic { get; }
  }
}