using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Features.Field.Mono;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Features.Field
{
  public class FieldAnimationHelper
  {
    private readonly int _fieldSizeX;
    private readonly int _fieldSizeY;
    private readonly SymbolCell[,] _cells;

    private readonly Action _onEnd;

    public FieldAnimationHelper(SymbolCell[,] cells, Action onEnd)
    {
      _onEnd = onEnd;
      
      _cells = cells;
      _fieldSizeX = _cells.GetLength(0);
      _fieldSizeY = _cells.GetLength(1);

      var listSubsequences = new List<Action>()
      {
        InQueue,
        InBackQueue,
        InVerticalQueue,
        InBackVerticalQueue,
        ToEachOther
      };
      
      listSubsequences[Random.Range(0, listSubsequences.Count)]?.Invoke();
      OnEndAnimation();
    }

    private async void OnEndAnimation()
    {
      await Task.Delay(TimeSpan.FromSeconds(1f));
      _onEnd?.Invoke();
    }

    private void InQueue()
    {
      for (var y = 0; y < _fieldSizeY; ++y)
      for (var x = 0; x < _fieldSizeX; ++x)
        _cells[y, x].ShowCell(y * _fieldSizeY + x);
    }

    private void InBackQueue()
    {
      var delay = 0;
      for (int y = _fieldSizeY - 1; y >= 0; y--)
      for (int x = _fieldSizeX - 1; x >= 0; x--)
        _cells[y, x].ShowCell(delay++);
    }

    private void InVerticalQueue()
    {
      for (var x = 0; x < _fieldSizeX; ++x)
      for (var y = 0; y < _fieldSizeY; ++y)
        _cells[y, x].ShowCell(x * _fieldSizeY + y);
    }

    private void InBackVerticalQueue()
    {
      var delay = 0;
      for (int x = _fieldSizeX - 1; x >= 0; x--)
      for (int y = _fieldSizeY - 1; y >= 0; y--)
        _cells[y, x].ShowCell(delay++);
    }

    private void ToEachOther()
    {
      var delay = 0;

      int centerX = _fieldSizeX / 2;
      int centerY = _fieldSizeY / 2;

      for (var y = 0; y <= centerY; y++)
      {
        for (var x = 0; x <= centerX; x++)
        {
          int mirroredRow = _fieldSizeX - 1 - y;
          int mirroredCol = _fieldSizeY - 1 - x;
          
          _cells[y, x].ShowCell(delay);
          _cells[mirroredRow, mirroredCol].ShowCell(delay);
          delay++;

          if (x == mirroredRow && y == mirroredCol) 
            continue;
          
          _cells[mirroredRow, y].ShowCell(delay);
          _cells[x, mirroredCol].ShowCell(delay);
        }
      }
    }
  }
}