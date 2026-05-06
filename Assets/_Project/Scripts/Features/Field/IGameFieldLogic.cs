using System;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.Field
{
  public interface IGameFieldLogic
  {
    void Construct(Action<SymbolSign> onEndGame);
    void Show(Action onEnd);
    SymbolCell[,] GetField();
  }
}