using System;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class ClassicBot
  {
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    private readonly IGameFactory _gameFactory;

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

      onEndTurn += () => symbolCells[GetBestMove(board)].SetSymbol(SymbolSign.O);

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
      int bestMove = -1;
      var bestScore = int.MinValue;
      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = 1;
        int score = -MiniMax(newBoard, -1);
        if (score <= bestScore) continue;
        bestScore = score;
        bestMove = i;
      }

      Debug.Log(g);
      return bestMove;
    }

    private int MiniMax(int[] board, int player)
    {
      int winner = CheckWinner(board);
      if (winner != 0)
        return winner * player;

      var bestScore = int.MinValue;
      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = -player;
        int score = -MiniMax(newBoard, player);
        if (score > bestScore) 
          bestScore = score;
      }

      if (bestScore == int.MinValue)
        return 0; // ничья

      return bestScore;
    }

    private int g = 0;
    private int CheckWinner(int[] board)
    {
      var lines = new[,]
      {
        {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // горизонтали
        {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // вертикали
        {0, 4, 8}, {2, 4, 6} // диагонали
      };
      g++;
      for (var i = 0; i < 8; i++)
      {
        int sum = board[lines[i, 0]] + board[lines[i, 1]] + board[lines[i, 2]];

        switch (sum)
        {
          case 3:
            return 1; // крестики выиграли
          case -3:
            return -1; // нолики выиграли
        }
      }

      return board.ToList().Contains(0) ? 0 : 2;
    }
  }
}