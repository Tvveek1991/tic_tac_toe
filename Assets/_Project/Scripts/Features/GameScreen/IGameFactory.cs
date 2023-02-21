using _Project.Scripts.Features.Field;
using _Project.Scripts.Features.Player;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    void CreateScreen();
    void CreateField();
    void SpawnPlayerObject();
    void ClearScreen();
    IGameFieldLogic GameFieldLogic { get; }
    IGameViewLogic GameViewLogic { get; }
  }
}