using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Bots
{
  public class Bot
  {
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    protected List<int> BestTurns;
    protected Dictionary<int, Potential> BestScores;

    protected readonly List<int[]> Lines = new List<int[]>
    {
      new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // horizontals
      new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // verticals
      new[] {0, 4, 8}, new[] {2, 4, 6} // diagonals
    };

    private readonly IGameFactory _gameFactory;
    private readonly IGameMetaProvider _gameMetaProvider;

    protected Bot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      _gameFactory = gameFactory;
      _gameMetaProvider = gameMetaProvider;
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
        int bestMove = GetBestMove(board, symbolCells);

        if (bestMove >= 0)
          symbolCells[bestMove].SetSymbol(new DragableData(SymbolSign.O, SymbolSize.Default,
            _gameMetaProvider.GetPlayerData()[SymbolSign.O].SymbolData.MeshClassic, _gameMetaProvider.GetPlayerData()[SymbolSign.O].SymbolData.MaterialClassic));
        else
          Debug.Log("GameIsOver as no winner");
      };

      Wait(onEndTurn);
    }

    protected virtual int GetBestMove(int[] board, SymbolCell[] symbolCells)
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