using _Project.Scripts.Features.Games.Classic;
using _Project.Scripts.Features.Player;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    void CreateScreen();
    void CreateField();
    void SpawnPlayerObject();
    void ClearScreen();
    IClassicFieldLogic BaseFieldLogic { get; }
    IGameViewLogic GameViewLogic { get; }
  }
}