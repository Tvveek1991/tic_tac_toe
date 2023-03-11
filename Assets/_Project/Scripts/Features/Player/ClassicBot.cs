using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Player
{
  public class ClassicBot
  {
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    private readonly IGameFactory _gameFactory;

    private List<int> _bestTurns;
    private Dictionary<int, Potential> _bestScores;

    private readonly List<int[]> _lines = new List<int[]>
    {
      new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // горизонтали
      new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // вертикали
      new[] {0, 4, 8}, new[] {2, 4, 6} // диагонали
    };

    public ClassicBot(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;
    }

    public void BotTurn(Action onEndTurn)
    {
      var symbolCells = new SymbolCell[_gameFactory.GameFieldLogic.GetField().Length];
      var board = new int[_gameFactory.GameFieldLogic.GetField().Length];

      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = _gameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = GetNumbForEvaluate(_gameFactory.GameFieldLogic.GetField()[y, x].GetSymbol());
      }

      onEndTurn += () =>
      {
        int bestMove = GetBestMove(board);

        if (bestMove >= 0)
          symbolCells[bestMove].SetSymbol(SymbolSign.O);
        else
          Debug.Log("GameIsOver as no winner");
      };

      Wait(onEndTurn);
    }

    private int GetNumbForEvaluate(SymbolSign symbolSign) =>
      symbolSign switch
      {
        SymbolSign.X => 1,
        SymbolSign.O => -1,
        _ => 0
      };

    private async void Wait(Action onEndTurn)
    {
      await Task.Delay(TimeSpan.FromSeconds(1f));
      onEndTurn?.Invoke();
    }

    private int GetBestMove(int[] board)
    {
      _bestScores = new Dictionary<int, Potential>();

      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = -1;

        if (_lines.Any(item => newBoard[item[0]] == -1 && newBoard[item[1]] == -1 && newBoard[item[2]] == -1))
        {
          _bestScores.Add(i, Potential.Winner);
          Debug.Log("Win turn_" + i);
          return i;
        }

        Debug.Log("No Win_" + i);

        CheckLose(board);

        foreach (int[] item in _lines)
        {
          if ((newBoard[item[0]] == -1 && newBoard[item[1]] == -1 && newBoard[item[2]] != 1
               || newBoard[item[0]] != 1 && newBoard[item[1]] == -1 && newBoard[item[2]] == -1
               || newBoard[item[0]] == -1 && newBoard[item[1]] != 1 && newBoard[item[2]] == -1) && !_bestScores.ContainsKey(i))
            _bestScores.Add(i, Potential.Big);
        }

        foreach (int[] item in _lines)
        {
          if ((newBoard[item[0]] != 1 && newBoard[item[1]] != 1 && newBoard[item[2]] != 1) && !_bestScores.ContainsKey(i))
            _bestScores.Add(i, Potential.Small);
        }
      }

      ;
      if (FindAllPotentialTurns(Potential.Lose))
        return _bestTurns[Random.Range(0, _bestTurns.Count)];

      Debug.Log("No Lose");

      if (FindAllPotentialTurns(Potential.Big))
        return _bestTurns[Random.Range(0, _bestTurns.Count)];

      if (FindAllPotentialTurns(Potential.Small))
        return _bestTurns[Random.Range(0, _bestTurns.Count)];

      return -1;
    }

    private bool FindAllPotentialTurns(Potential potential)
    {
      _bestTurns = (from item in _bestScores where item.Value == potential select item.Key).ToList();
      Debug.Log(potential + " potential found: " + _bestTurns.Count);
      
      return _bestTurns.Count > 0;
    }

    private void CheckLose(int[] board)
    {
      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = 1;

        if (!_lines.Any(item => newBoard[item[0]] == 1 && newBoard[item[1]] == 1 && newBoard[item[2]] == 1))
          continue;

        if (!_bestScores.ContainsKey(i))
          _bestScores.Add(i, Potential.Lose);
        else
          _bestScores[i] = Potential.Lose;
        
        Debug.Log("Lose turn_" + i);
      }
    }

    private enum Potential
    {
      Small,
      Big,
      Lose,
      Winner,
    }
  }
}