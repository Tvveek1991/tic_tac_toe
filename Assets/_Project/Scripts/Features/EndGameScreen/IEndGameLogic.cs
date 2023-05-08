using System;

namespace _Project.Scripts.Features.EndGameScreen
{
  public interface IEndGameLogic
  {
    void Construct(Action onClickHome, Action onClickPlayAgain);
    void Show();
  }
}