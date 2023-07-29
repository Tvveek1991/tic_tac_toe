using System;
using System.Collections.Generic;
using _Project.Scripts.Features.GameScreen.Serialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.EndGameScreen.Mono
{
  public class EndGameMono : MonoBehaviour
  {
    public Button ButtonHome;
    public Button ButtonPlayAgain;

    public CanvasGroup CanvasGroup;

    public List<Result> ResultList;

    public GameObject ScorePanel;
    
    public TextMeshProUGUI BestWinStreakText;
    public TextMeshProUGUI WinStreakText;

    [Space(20)]
    public Image ResultIcon;
    public Sprite WinIcon;
    public Sprite LoseIcon;
    public Sprite DrawIcon;

    [Serializable]
    public class Result
    {
      public SymbolSign SymbolSign;
      public Image Icon;
      public TextMeshProUGUI Score;
    }
  }
}