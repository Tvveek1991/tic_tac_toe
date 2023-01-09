using _Project.Scripts.Features.GameScreen;
using _Project.Scripts.Features.GameScreen.Serialize;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Games.Classic
{
  public class ClassicFieldLogic : BaseFieldLogic
  {
    private const string ClassicFieldPath = "Features/GameScreen/Fields/ClassicField";

    private const int ToWin = 3;
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;
    
    private SymbolSign[,] _field = new SymbolSign[3, 3];

    public ClassicFieldLogic(IGameFactory gameFactory, IAudioService audioService) : base(audioService)
    {
      AudioService = audioService;

      GameField = gameFactory.CreateField(ClassicFieldPath);
      
      GameField.ChangeSignButton.onClick.AddListener(() =>
      {
        Debug.Log(CheckWin(SymbolSign.X));
        _field[Random.Range(0, 3), Random.Range(0, 3)] = SymbolSign.X;
      });
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
        Debug.Log(_field[y + i * vy, x + i * vx]);
        if (_field[y + i * vy, x + i * vx] != sign)
          return false;
      }

      return true;
    }

    private bool IsValidCell(int farX, int farY)
      => ((farX >= 0) && (farX <= FieldSizeX)) && ((farY >= 0) && (farY <= FieldSizeY));  
  }
}