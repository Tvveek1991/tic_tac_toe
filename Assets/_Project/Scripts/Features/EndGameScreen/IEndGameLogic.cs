using System;
using _Project.Scripts.Features.GameScreen.Serialized;

namespace _Project.Scripts.Features.EndGameScreen
{
  public interface IEndGameLogic
  {
    void Construct(Action onClickHome, Action onClickPlayAgain);
    void Show(SymbolSign symbolSign);
  }
}