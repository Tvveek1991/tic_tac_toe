using _Project.Scripts.Features.EndGameScreen;
using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.GameScreen.Mono;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    WarningView WarningView { get; }
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