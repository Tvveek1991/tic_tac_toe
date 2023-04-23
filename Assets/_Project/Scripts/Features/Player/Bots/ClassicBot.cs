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
  public class ClassicBot : Bot
  {
    public ClassicBot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider) : base(gameFactory, gameMetaProvider)
    {
      DragableData = new DragableData(SymbolSign.O, SymbolSize.Default,
        gameMetaProvider.GetPlayerData()[SymbolSign.O].SymbolData.MeshClassic, gameMetaProvider.GetPlayerData()[SymbolSign.O].SymbolData.MaterialClassic);
    }

    protected override int GetBestMove(int[] board, SymbolCell[] symbolCells)
    {
      BestScores = new Dictionary<int, Potential>();

      for (var i = 0; i < 9; i++)
      {
        if (board[i] != 0)
          continue;

        var newBoard = (int[]) board.Clone();
        newBoard[i] = -1;

        if (Lines.Any(item => newBoard[item[0]] == -1 && newBoard[item[1]] == -1 && newBoard[item[2]] == -1))
        {
          if (!BestScores.ContainsKey(i))
            BestScores.Add(i, Potential.Winner);
          else
            BestScores[i] = Potential.Winner;
          // Debug.Log("Win turn_" + i);
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
          if (new[] {newBoard[item[0]], newBoard[item[1]], newBoard[item[2]]}.All(x => x != 1) && !BestScores.ContainsKey(i))
            BestScores.Add(i, Potential.Small);
        }
        
        /*foreach (int[] item in Lines)
        {
          if ((newBoard[item[0]] == -1 && newBoard[item[1]] == -1 && newBoard[item[2]] != 1
               || newBoard[item[0]] != 1 && newBoard[item[1]] == -1 && newBoard[item[2]] == -1
               || newBoard[item[0]] == -1 && newBoard[item[1]] != 1 && newBoard[item[2]] == -1) && !BestScores.ContainsKey(i))
            BestScores.Add(i, Potential.Big);
        }

        foreach (int[] item in Lines)
        {
          if ((newBoard[item[0]] != 1 && newBoard[item[1]] != 1 && newBoard[item[2]] != 1) && !BestScores.ContainsKey(i))
            BestScores.Add(i, Potential.Small);
        }*/
      }

      if (FindAllPotentialTurns(Potential.Lose))
        return BestTurns[Random.Range(0, BestTurns.Count)];
      // Debug.Log("No Lose");

      if (FindAllPotentialTurns(Potential.Big))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      if (FindAllPotentialTurns(Potential.Small))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      return -1;
    }

    private bool FindAllPotentialTurns(Potential potential)
    {
      BestTurns = (from item in BestScores where item.Value == potential select item.Key).ToList();
      // Debug.Log(potential + " potential found: " + BestTurns.Count);

      return BestTurns.Count > 0;
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