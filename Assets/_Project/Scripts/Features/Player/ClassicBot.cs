using System;
using UnityEngine;

namespace _Project.Scripts.Features.Player
{
  public class ClassicBot
  {
    
    public ClassicBot()
    { }

    public void BotTurn(Action onEndTurn)
    {
      Debug.Log("Bot turned");
    }
  }
}