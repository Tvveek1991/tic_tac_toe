using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.Field.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

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
    private readonly IInputResolver _inputResolver;
    private readonly IGameMetaProvider _gameMetaProvider;

    private Action<SymbolSign> _onEndGame;

    private List<Transform> _wonCells;

    public GameFieldLogic(GameField gameField, IAudioService audioService, IGameMetaProvider gameMetaProvider, IInputResolver inputResolver)
    {
      _gameField = gameField;
      _audioService = audioService;
      _inputResolver = inputResolver;
      _gameMetaProvider = gameMetaProvider;

      SetField();
      CreateCellsField();
    }

    public void Construct(Action<SymbolSign> onEndGame) =>
      _onEndGame = onEndGame;

    public void Show(Action onEnd) =>
      ShowCells(onEnd);

    public SymbolCell[,] GetField() =>
      _cells;

    private void CreateCellsField()
    {
      for (var y = 0; y < FieldSizeY; ++y)
      for (var x = 0; x < FieldSizeX; ++x)
      {
        _cells[y, x] = _gameField.GetPlayablePoint[y * FieldSizeY + x];

        _cells[y, x].Construct(_audioService, _gameMetaProvider,
          delegate(DragableData dragableData)
          {
            _playerSymbol = dragableData.GetSymbol();

            if (CheckWin(_playerSymbol) || CheckDraw())
            {
              _inputResolver.Disable();
              
              var winAnimation = _gameField.gameObject.AddComponent<WinAnimation>();
              winAnimation.Construct(_wonCells, () => { _onEndGame?.Invoke(CheckWin(_playerSymbol) ? _playerSymbol : SymbolSign.None); },
                _playerSymbol == _gameMetaProvider.GetPlayerView().GetSymbol());
            }
          });
      }
    }

    private void ShowCells(Action onEnd) =>
      new FieldAnimationHelper(_cells, onEnd);

    private void SetField() =>
      _gameField.SetField(_gameMetaProvider.GetMap());

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

      _wonCells = new List<Transform>();

      for (var i = 0; i < len; i++)
      {
        if (_cells[y + i * vy, x + i * vx].GetSymbol() != sign)
          return false;

        _wonCells.Add(_cells[y + i * vy, x + i * vx].transform);
      }

      return true;
    }

    private bool IsValidCell(int farX, int farY)
      => ((farX >= 0) && (farX < FieldSizeX)) && ((farY >= 0) && (farY < FieldSizeY));
  }
}