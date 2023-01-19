using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Games.Classic
{
  public class ClassicFieldLogic : BaseFieldLogic, IClassicFieldLogic
  {
    private const int ToWin = 3;
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    private readonly SymbolCell[,] _cells = new SymbolCell[FieldSizeY, FieldSizeX];

    public ClassicFieldLogic(GameFieldView gameFieldView, IAudioService audioService) : base(gameFieldView, audioService)
    {
    }

    public void Construct()
    {
      CreateCellsField();
    }

    public void SetFieldCell(SymbolSign symbol)
    {
      _cells[Random.Range(0, 3), Random.Range(0, 3)].SetSymbol(symbol);
      Debug.Log(CheckWin(symbol));
    }

    private void CreateCellsField()
    {
      for (var y = 0; y < FieldSizeY; ++y)
      for (var x = 0; x < FieldSizeX; ++x)
        _cells[y, x] = GameFieldView.GetPlayablePoint[y * FieldSizeY + x];
    }

    private bool CheckWin(SymbolSign symbol)
    {
      for (var i = 0; i < FieldSizeX; i++)
      {
        for (var j = 0; j < FieldSizeY; j++)
        {
          if (CheckLine(i, j, 1, 0, ToWin, symbol))
            return true;
          if (CheckLine(i, j, 1, 1, ToWin, symbol))
            return true;
          if (CheckLine(i, j, 0, 1, ToWin, symbol))
            return true;
          if (CheckLine(i, j, 1, -1, ToWin, symbol))
            return true;
        }
      }

      return false;
    }

    private bool CheckLine(int x, int y, int vx, int vy, int len, SymbolSign sign)
    {
      int farX = x + (len - 1) * vx;
      int farY = y + (len - 1) * vy;

      if (!IsValidCell(farX, farY))
        return false;

      for (var i = 0; i < len; i++)
      {
        if (_cells[y + i * vy, x + i * vx].GetSymbol != sign)
          return false;
      }

      return true;
    }

    private bool IsValidCell(int farX, int farY)
      => ((farX >= 0) && (farX <= FieldSizeX)) && ((farY >= 0) && (farY <= FieldSizeY));
  }
}