using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.Bots.Serialized;
using _Project.Scripts.Services._Interfaces;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Player.Bots
{
  public class BuhurtBot : Bot
  {
    private Dictionary<SymbolSize, Dictionary<int, Potential>> _sizeChecking;
    private readonly SymbolSize[] _figures = {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large};

    public BuhurtBot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider) : base(gameFactory, gameMetaProvider)
    {
    }

    public override bool IsFiguresOver() =>
      _figures.All(item => item == SymbolSize.Default);

    protected override int GetBestMove(SymbolSign[] board, SymbolCell[] symbolCells)
    {
      _sizeChecking = new Dictionary<SymbolSize, Dictionary<int, Potential>>();

      for (var size = 0; size < Enum.GetValues(typeof(SymbolSize)).Length - 1; size++)
      {
        BestScores = new Dictionary<int, Potential>();
        for (var cell = 0; cell < 9; cell++)
        {
          if (board[cell] == BotSign || symbolCells[cell].GetSize() >= _figures[size] || _figures[size] == SymbolSize.Default)
            continue;

          var newBoard = (SymbolSign[]) board.Clone();
          newBoard[cell] = BotSign;

          if (CheckWin(newBoard, cell, out int winMove))
            return winMove;

          CheckLose(board);

          AddPotentialTurns(newBoard, cell);
        }

        _sizeChecking.Add((SymbolSize) size, BestScores);
      }

      if (BestScores.Count == 0)
        return -1;

      if (FindAllPotentialTurns(Potential.Lose))
        return ChooseSizeTurn();

      if (FindAllPotentialTurns(Potential.Big))
        return ChooseSizeTurn();

      if (FindAllPotentialTurns(Potential.Small))
        return ChooseSizeTurn();

      return -1;
    }

    protected override bool FindAllPotentialTurns(Potential potential)
    {
      var collectBestTurns = new List<int>();

      foreach (KeyValuePair<SymbolSize, Dictionary<int, Potential>> t in _sizeChecking)
        collectBestTurns.AddRange(from item in t.Value where item.Value == potential select item.Key);

      BestTurns = collectBestTurns;
      // Debug.Log(potential + " potential found: " + BestTurns.Count);

      return BestTurns.Count > 0;
    }

    private int ChooseSizeTurn()
    {
      int rnd = BestTurns[Random.Range(0, BestTurns.Count)];

      var sizeTurn = new List<SymbolSize>();
      foreach (KeyValuePair<SymbolSize, Dictionary<int, Potential>> item in _sizeChecking)
      {
        if (item.Value.ContainsKey(rnd) && item.Key != SymbolSize.Default)
          sizeTurn.Add(item.Key);
      }

      Debug.Log(sizeTurn.Count);
      sizeTurn.ForEach(item => Debug.Log(item));

      SymbolSize size = sizeTurn[Random.Range(0, sizeTurn.Count)];

      DragableData = new DragableData(BotSign, size);

      _figures[(int) size] = SymbolSize.Default;
      _figures.ForEach(item => Debug.Log(item));

      return rnd;
    }
  }
}