using _Project.Scripts.Features.EndGameScreen;
using _Project.Scripts.Features.Field;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    void CreateScreen();
    void CreateField();
    void CreateEndGame();
    void SpawnPlayerObject();
    void ClearScreen();
    IGameFieldLogic GameFieldLogic { get; }
    IGameViewLogic GameViewLogic { get; }
    IEndGameLogic EndGameLogic { get; }
  }
}