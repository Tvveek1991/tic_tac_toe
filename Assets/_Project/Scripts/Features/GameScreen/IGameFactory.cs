using _Project.Scripts.Features.Games.Classic.Mono;
using _Project.Scripts.Features.GameScreen.Mono;
using UnityEngine;

namespace _Project.Scripts.Features.GameScreen
{
  public interface IGameFactory
  {
    GameView CreatViewScreen();
    GameFieldView CreateField(string path);
  }
}