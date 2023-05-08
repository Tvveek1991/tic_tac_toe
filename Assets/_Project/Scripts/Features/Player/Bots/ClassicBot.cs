using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Bots
{
  public class ClassicBot : Bot
  {
    public ClassicBot(IGameFactory gameFactory) : base(gameFactory) =>
      DragableData = new DragableData(SymbolSign.O, SymbolSize.Default);

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
      }

      Debug.Log(Difficult);
      if (CheckDifficult(out int bestMove))
        return bestMove;

      if (FindAllPotentialTurns(Potential.Lose))
        return BestTurns[Random.Range(0, BestTurns.Count)];
      // Debug.Log("No Lose");

      if (FindAllPotentialTurns(Potential.Big))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      if (FindAllPotentialTurns(Potential.Small))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      return -1;
    }

    private bool CheckDifficult(out int bestMove)
    {
      switch (Difficult)
      {
        case BotDifficult.Medium:
          int rnd = Random.Range(0, 2);
          if (rnd == 0)
            break;
          else
          {
            GetRandomTurn(out bestMove);
            return true;
          }

        case BotDifficult.Easy:
        {
          GetRandomTurn(out bestMove);
          return true;
        }

        case BotDifficult.Hard:
          break;
      }

      bestMove = 0;
      return false;
    }

    private void GetRandomTurn(out int bestMove)
    {
      List<int> list = (from item in BestScores select item.Key).ToList();
      bestMove = list[Random.Range(0, list.Count)];
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