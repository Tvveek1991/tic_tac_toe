using System.Collections.Generic;
using _Project.Scripts.Features.Player.BuhurtModules;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameViewMono : MonoBehaviour
  {
    [Space(20)] [SerializeField] private Button backButton;

    [SerializeField] private SymbolOperatorUI playerPart;
    [SerializeField] private SymbolOperatorUI opponentPart;

    [Header("Buhurt UI")] 
    [SerializeField] private CanvasGroup playerFiguresCanvasGroup;
    [SerializeField] private List<PlayerFigure> playerFigures;
    
    private readonly int _netPlayersCount;

    public Button BackButton => backButton;
    public SymbolOperatorUI PlayerPart => playerPart;
    public SymbolOperatorUI OpponentPart => opponentPart;

    public List<PlayerFigure> GetFigures() =>
      playerFigures;

    public void SetVisibleBuhurtPart(bool isOn)
    {
      playerFigures.ForEach(item => item.SetVisibility(isOn));
      
      playerFiguresCanvasGroup.DOFade(isOn ? 1f : 0f, .5f);
      playerFiguresCanvasGroup.blocksRaycasts = isOn;
    }
  }
}