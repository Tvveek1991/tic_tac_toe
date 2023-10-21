using System;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.Field
{
  public class GameFieldLogic : IGameFieldLogic
  {
    private const int ToWin = 3;
    private const int FieldSizeX = 3;
    private const int FieldSizeY = 3;

    private readonly SymbolCell[,] _cells = new SymbolCell[FieldSizeY, FieldSizeX];

    private SymbolSign _playerSymbol;

    private readonly GameField _gameField;

    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    private Action<SymbolSign> _onEndGame;

    public GameFieldLogic(GameField gameField, IAudioService audioService, IGameMetaProvider gameMetaProvider)
    {
      _gameField = gameField;
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;

      SetField();
      CreateCellsField();
      ShowCells();
    }

    public void Construct(Action<SymbolSign> onEndGame)
    {
      _onEndGame = onEndGame;
    }

    public SymbolCell[,] GetField() =>
      _cells;
    
    private void CreateCellsField()
    {
      for (var y = 0; y < FieldSizeY; ++y)
      for (var x = 0; x < FieldSizeX; ++x)
      {
        _cells[y, x] = _gameField.GetPlayablePoint[y * FieldSizeY + x];
        int x1 = x;
        int y1 = y;
        _cells[y, x].Construct(_audioService,
          delegate(DragableData dragableData)
          {
            _playerSymbol = dragableData.GetSymbol();
            SetFieldCell(x1, y1, dragableData);

            if (CheckWin(_playerSymbol) || CheckDraw())
              _onEndGame?.Invoke(CheckWin(_playerSymbol) ? _playerSymbol : SymbolSign.None);
          });
      }
    }

    private void ShowCells() => 
      new FieldAnimationHelper(_cells);

    private void SetField() =>
      _gameField.SetField(_gameMetaProvider.GetMap());

    private void SetFieldCell(int x, int y, DragableData dragableData)
    {
      if (_cells[y, x] == null)
        return;

      _cells[y, x].MeshFilter.mesh = _gameMetaProvider.GetMesh(dragableData);
      _cells[y, x].MeshRenderer.material = _gameMetaProvider.GetMaterial(dragableData);
      _cells[y, x].MeshRenderer.enabled = true;

      // Debug.Log("Win is " + CheckWin(_playerSymbol) + (CheckWin(_playerSymbol) ? " Is" + _playerSymbol : ""));
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

    private bool CheckDraw() =>
      _gameMetaProvider.GetGame() != Game.Buhurt && _gameField.GetPlayablePoint.All(t => t.GetSymbol() != SymbolSign.None);

    private bool CheckLine(int x, int y, int vx, int vy, int len, SymbolSign sign)
    {
      int farX = x + (len - 1) * vx;
      int farY = y + (len - 1) * vy;

      if (!IsValidCell(farX, farY))
        return false;

      for (var i = 0; i < len; i++)
      {
        if (_cells[y + i * vy, x + i * vx].GetSymbol() != sign)
          return false;
      }

      return true;
    }

    private bool IsValidCell(int farX, int farY)
      => ((farX >= 0) && (farX < FieldSizeX)) && ((farY >= 0) && (farY < FieldSizeY));
  }
}