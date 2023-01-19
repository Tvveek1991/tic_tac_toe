using _Project.Scripts.Features.Games.Classic;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    void CreateScreen();
    void CreateField();
    void ClearScreen();
    IClassicFieldLogic BaseFieldLogic { get; }
    IGameViewLogic GameViewLogic { get; }
  }
}