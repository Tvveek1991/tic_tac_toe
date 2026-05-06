using System.Collections.Generic;
using _Project.Scripts.Features.Player.BuhurtModules;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.GameScreen.Mono
{
  public class GameViewMono : MonoBehaviour
  {
    [Space(20)]
    [SerializeField]  private Button backButton;
    [SerializeField]  private Button tutorialButton;

    [SerializeField] private SymbolOperatorUI playerPart;
    [SerializeField] private SymbolOperatorUI opponentPart;

    [Header("Buhurt UI")] 
    [SerializeField] private CanvasGroup playerCanvasGroup;
    [SerializeField] private List<PlayerFigure> playerFigures;
    [SerializeField] private CanvasGroup opponentCanvasGroup;
    [SerializeField] private List<PlayerFigure> opponentFigures;

    public Button BackButton => backButton;
    public Button TutorialButton => tutorialButton;
    public CanvasGroup PlayerCanvasGroup => playerCanvasGroup;
    public CanvasGroup OpponentCanvasGroup => opponentCanvasGroup;

    public SymbolOperatorUI GetSymbolPart(bool isOpponent = false) => 
      isOpponent ? opponentPart : playerPart;
    
    public List<PlayerFigure> GetFigures(bool isOpponent = false) =>
      isOpponent ? opponentFigures : playerFigures;
  }
}