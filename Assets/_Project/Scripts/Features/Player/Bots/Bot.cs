using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Player.Bots
{
  public class Bot
  {
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    protected List<int> BestTurns;
    protected Dictionary<int, Potential> BestScores;

    protected readonly BotDifficult Difficult;
    
    protected DragableData DragableData;

    protected readonly List<int[]> Lines = new List<int[]>
    {
      new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // horizontals
      new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // verticals
      new[] {0, 4, 8}, new[] {2, 4, 6} // diagonals
    };

    private readonly int[][] _bigPotentialCase =
    {
      new[] {-1, -1, 1},
      new[] {1, -1, -1},
      new[] {-1, 1, -1}
    };

    private readonly IGameFactory _gameFactory;


    protected Bot(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;

      Difficult = (BotDifficult)Random.Range(0, 3);
      Debug.Log("Bot difficult is " + Difficult);
    }

    public void BotTurn(Action onEndTurn, Action onEndGame)
    {
      var symbolCells = new SymbolCell[_gameFactory.GameFieldLogic.GetField().Length];
      var board = new int[_gameFactory.GameFieldLogic.GetField().Length];

      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = GetNumbForEvaluate(_gameFactory.GameFieldLogic.GetField()[y, x].GetSymbol());
      }

      Wait(onEndTurn, () =>
      {
        int bestMove = GetBestMove(board, symbolCells);

        if (bestMove >= 0)
          symbolCells[bestMove].SetSymbol(DragableData);
        else
          onEndGame?.Invoke();
      });
    }

    public virtual bool IsFiguresOver() =>
      false;

    protected virtual int GetBestMove(int[] board, SymbolCell[] symbolCells)
    {
      Debug.Log("Wrong Class");
      return -1;
    }

    protected bool CheckBigPotential(int[] board, int[] line)
    {
      int[] check = {board[line[0]], board[line[1]], board[line[2]]};
      return _bigPotentialCase.Any(item => item == check);
    }

    private async void Wait(Action onEndTurn, Action makeTurn)
    {
      await Task.Delay(TimeSpan.FromSeconds(1f));
      makeTurn?.Invoke();
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

    protected enum BotDifficult
    {
      Easy,
      Medium,
      Hard
    }
  }
}