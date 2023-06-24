using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Features.Player.Bots.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Player.Bots
{
  public class ClassicBot : Bot
  {
    public ClassicBot(IGameFactory gameFactory, IGameMetaProvider gameMetaProvider) : base(gameFactory, gameMetaProvider)
    {
    }

    protected override int GetBestMove(SymbolSign[] board, SymbolCell[] symbolCells)
    {
      BestScores = new Dictionary<int, Potential>();
      DragableData = new DragableData(BotSign, SymbolSize.Default);

      for (var cell = 0; cell < 9; cell++)
      {
        if (board[cell] != 0)
          continue;

        var newBoard = (SymbolSign[]) board.Clone();
        newBoard[cell] = BotSign;

        if (CheckWin(newBoard, cell, out int winMove)) 
          return winMove;

        CheckLose(board);

        AddPotentialTurns(newBoard, cell);
      }

      if (BestScores.Count == 0)
        return -1;

      if (CheckDifficult(out int bestMove))
        return bestMove;

      if (FindAllPotentialTurns(Potential.Lose))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      if (FindAllPotentialTurns(Potential.Big))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      if (FindAllPotentialTurns(Potential.Small))
        return BestTurns[Random.Range(0, BestTurns.Count)];

      return -1;
    }

    private bool FindAllPotentialTurns(Potential potential)
    {
      BestTurns = (from item in BestScores where item.Value == potential select item.Key).ToList();
      return BestTurns.Count > 0;
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
  }
}