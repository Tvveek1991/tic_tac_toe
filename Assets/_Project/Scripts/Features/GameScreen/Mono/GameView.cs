using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameView : MonoBehaviour
  {
    public Button BackButton;
    
    public void Construct(Action onBack)
    {
      BackButton.onClick.AddListener(() => onBack?.Invoke());
    }
  }
}