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

    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private TextMeshProUGUI loadProgress;

    [Header("Player UI")] 
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Image symbolImage;

    [Header("Buhurt UI")] 
    [SerializeField] private CanvasGroup playerFiguresCanvasGroup;
    [SerializeField] private List<PlayerFigure> playerFigures;
    
    private readonly int _netPlayersCount;

    public Button BackButton => backButton;
    public TextMeshProUGUI TurnText => turnText;
    public Image SymbolImage => symbolImage;

    public void HideLoadScreen() =>
      loadScreen.DOFade(0, .5f).OnComplete(() => loadScreen.blocksRaycasts = false);

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