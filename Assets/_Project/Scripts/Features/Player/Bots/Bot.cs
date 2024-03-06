using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.Bots.Serialized;
using _Project.Scripts.Services._Interfaces;
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

    protected SymbolSign BotSign;

    protected readonly BotDifficult Difficult;

    protected DragableData DragableData;

    protected bool IsWinnTurn;
    
    protected readonly IGameFactory GameFactory;

    private readonly List<int[]> _lines = new()
    {
      new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // horizontals
      new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // verticals
      new[] {0, 4, 8}, new[] {2, 4, 6} // diagonals
    };

    private SymbolSign _playerSign;
    private SymbolSign[][] _bigPotentialCase;

    private readonly IGameMetaProvider _gameMetaProvider;


    protected Bot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider)
    {
      GameFactory = gameFactory;
      _gameMetaProvider = gameMetaProvider;

      Difficult = (BotDifficult) Random.Range(0, 3);
      Debug.Log("Bot difficult is " + Difficult);
    }

    public void Construct()
    {
      _playerSign = _gameMetaProvider.GetPlayerView().GetSymbol();
      BotSign = _playerSign == SymbolSign.X ? SymbolSign.O : SymbolSign.X;

      _gameMetaProvider.CreateBotData(BotSign);
      
      _bigPotentialCase = new[] {
        new[] {BotSign, BotSign, _playerSign},
        new[] {_playerSign, BotSign, BotSign},
        new[] {BotSign, _playerSign, BotSign}
      };
    }

    public async void BotTurn(Action onEndTurn, Action onEndGame)
    {
      var symbolCells = new SymbolCell[GameFactory.GameFieldLogic.GetField().Length];
      var board = new SymbolSign[GameFactory.GameFieldLogic.GetField().Length];

      for (var y = 0; y < FieldSizeY; y++)
      for (var x = 0; x < FieldSizeX; x++)
      {
        symbolCells[3 * y + x] = GameFactory.GameFieldLogic.GetField()[y, x];
        board[3 * y + x] = GameFactory.GameFieldLogic.GetField()[y, x].GetSymbol();
      }

      await Wait(() => {
        int bestMove = GetBestMove(board, symbolCells);

        if (bestMove >= 0)
          symbolCells[bestMove].SetSymbol(DragableData);
        else
          onEndGame?.Invoke();
        
      });
      
      if(IsWinnTurn)
        return;
      
      onEndTurn?.Invoke();
    }

    protected virtual int GetBestMove(SymbolSign[] board, SymbolCell[] symbolCells) =>
      -1;

    protected void CheckWin(SymbolSign[] newBoard, int cell)
    {
      if (!_lines.Any(item => newBoard[item[0]] == BotSign && newBoard[item[1]] == BotSign && newBoard[item[2]] == BotSign))
        return;

      if (!BestScores.ContainsKey(cell))
        BestScores.Add(cell, Potential.Winner);
      else
        BestScores[cell] = Potential.Winner;
    }

    protected void CheckLose(SymbolSign[] board)
    {
      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (SymbolSign[]) board.Clone();
        newBoard[i] = _playerSign;

        if (!_lines.Any(item => newBoard[item[0]] == _playerSign && newBoard[item[1]] == _playerSign && newBoard[item[2]] == _playerSign))
          continue;

        if (!BestScores.ContainsKey(i))
          BestScores.Add(i, Potential.Lose);
        else
          BestScores[i] = Potential.Lose;
      }
    }

    protected void AddPotentialTurns(SymbolSign[] newBoard, int i)
    {
      foreach (int[] item in _lines)
      {
        if (CheckBigPotential(newBoard, item) && !BestScores.ContainsKey(i))
          BestScores.Add(i, Potential.Big);
      }

      foreach (int[] item in _lines)
      {
        if (CheckSmallPotential(newBoard, item) && !BestScores.ContainsKey(i))
          BestScores.Add(i, Potential.Small);
      }
    }

    private async Task Wait(Action makeTurn)
    {
      await Task.Delay(TimeSpan.FromSeconds(1f));
      makeTurn?.Invoke();
    }

    private bool CheckBigPotential(SymbolSign[] board, int[] line)
    {
      SymbolSign[] check = {board[line[0]], board[line[1]], board[line[2]]};
      return _bigPotentialCase.Any(item => item == check);
    }

    private bool CheckSmallPotential(SymbolSign[] newBoard, int[] item) =>
      new[] {newBoard[item[0]], newBoard[item[1]], newBoard[item[2]]}.All(x => x != _playerSign);
  }
}