using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Bots
{
  public class Bot
  {
    protected const int FieldSizeX = 3;
    protected const int FieldSizeY = 3;

    protected List<int> BestTurns;
    protected Dictionary<int, Potential> BestScores;

    protected readonly List<int[]> Lines = new List<int[]>
    {
      new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // горизонтали
      new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // вертикали
      new[] {0, 4, 8}, new[] {2, 4, 6} // диагонали
    };

    protected readonly IGameFactory GameFactory;

    protected Bot(IGameFactory gameFactory) =>
      GameFactory = gameFactory;

    public void BotTurn(Action onEndTurn)
    {
      var symbolCells = new SymbolCell[GameFactory.GameFieldLogic.GetField().Length];
      var board = new int[GameFactory.GameFieldLogic.GetField().Length];

      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = GameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = GetNumbForEvaluate(GameFactory.GameFieldLogic.GetField()[y, x].GetSymbol());
      }

      onEndTurn += () =>
      {
        int bestMove = GetBestMove(board);

        if (bestMove >= 0)
          symbolCells[bestMove].SetSymbol(SymbolSign.O, null);
        else
          Debug.Log("GameIsOver as no winner");
      };

      Wait(onEndTurn);
    }

    protected virtual int GetBestMove(int[] board)
    {
      Debug.Log("Wrong Class");
      return -1;
    }

    private async void Wait(Action onEndTurn)
    {
      await Task.Delay(TimeSpan.FromSeconds(1f));
      onEndTurn?.Invoke();
    }

    private int GetNumbForEvaluate(SymbolSign symbolSign) =>
      symbolSign switch
      {
        SymbolSign.X => 1,
        SymbolSign.O => -1,
        _ => 0
      };

    protected enum Potential
    {
      Small,
      Big,
      Lose,
      Winner,
    }
  }
}