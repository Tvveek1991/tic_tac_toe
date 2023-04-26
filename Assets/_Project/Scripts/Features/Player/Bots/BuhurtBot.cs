using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Bots
{
  public class BuhurtBot : Bot
  {
    private Dictionary<SymbolSize, Dictionary<int, Potential>> _sizeChecking;
    private readonly SymbolSize[] _figures = {SymbolSize.Tiny, SymbolSize.Small, SymbolSize.Medium, SymbolSize.Big, SymbolSize.Large};

    public BuhurtBot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider) : base(gameFactory, gameMetaProvider)
    {
    }

    protected override int GetBestMove(int[] board, SymbolCell[] symbolCells)
    {
      _sizeChecking = new Dictionary<SymbolSize, Dictionary<int, Potential>>();
      for (var size = 0; size < 5; size++)
      {
        BestScores = new Dictionary<int, Potential>();
        
        for (var i = 0; i < 9; i++)
        {
          if (board[i] == -1 || symbolCells[i].GetSize() >= _figures[size] || _figures[size] == SymbolSize.Default)
            continue;

          var newBoard = (int[]) board.Clone();
          newBoard[i] = -1;

          if (Lines.Any(item => newBoard[item[0]] == -1 && newBoard[item[1]] == -1 && newBoard[item[2]] == -1))
          {
            if (!BestScores.ContainsKey(i))
              BestScores.Add(i, Potential.Winner);
            else
              BestScores[i] = Potential.Winner;
            Debug.Log("Win turn_" + i);
            return i;
          }

          // Debug.Log("No Win_" + i);

          CheckLose(board);

          foreach (int[] item in Lines)
          {
            if (CheckBigPotential(newBoard, item) && !BestScores.ContainsKey(i))
              BestScores.Add(i, Potential.Big);
          }

          foreach (int[] item in Lines)
          {
            // if ((newBoard[item[0]] != 1 && newBoard[item[1]] != 1 && newBoard[item[2]] != 1) && !BestScores.ContainsKey(i))
            if (new[] {newBoard[item[0]], newBoard[item[1]], newBoard[item[2]]}.All(x => x != 1) && !BestScores.ContainsKey(i))
              BestScores.Add(i, Potential.Small);
          }
        }

        _sizeChecking.Add((SymbolSize) size, BestScores);
      }

      if (FindAllPotentialTurns(Potential.Lose))
        return ChooseSizeTurn();

      // Debug.Log("No Lose");

      if (FindAllPotentialTurns(Potential.Big))
        return ChooseSizeTurn();

      if (FindAllPotentialTurns(Potential.Small))
        return ChooseSizeTurn();

      return -1;
    }

    private bool FindAllPotentialTurns(Potential potential)
    {
      var collectBestTurns = new List<int>();

      foreach (KeyValuePair<SymbolSize, Dictionary<int, Potential>> t in _sizeChecking)
        collectBestTurns.AddRange(from item in t.Value where item.Value == potential select item.Key);

      BestTurns = collectBestTurns;
      Debug.Log(potential + " potential found: " + BestTurns.Count);

      return BestTurns.Count > 0;
    }

    private int ChooseSizeTurn()
    {
      int rnd = BestTurns[Random.Range(0, BestTurns.Count)];

      var sizeTurn = new List<SymbolSize>();
      foreach (KeyValuePair<SymbolSize, Dictionary<int, Potential>> t in _sizeChecking)
      {
        if (t.Value.ContainsKey(rnd) && t.Key != SymbolSize.Default)
          sizeTurn.Add(t.Key);
      }

      SymbolSize size = sizeTurn[Random.Range(0, sizeTurn.Count)];

      DragableData = new DragableData(SymbolSign.O, size);

      _figures[(int) size] = SymbolSize.Default;

      return rnd;
    }

    private void CheckLose(int[] board)
    {
      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0) continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = 1;

        if (!Lines.Any(item => newBoard[item[0]] == 1 && newBoard[item[1]] == 1 && newBoard[item[2]] == 1))
          continue;

        if (!BestScores.ContainsKey(i))
          BestScores.Add(i, Potential.Lose);
        else
          BestScores[i] = Potential.Lose;

        // Debug.Log("Lose turn_" + i);
      }
    }
  }
}