using System.Collections.Generic;
using _Project.Scripts.Features.Tutorial.Stages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Tutorial
{
  public class TutorialView : MonoBehaviour
  {
    public Button BackButton;

    public Button PreviousButton;
    public Button NextButton;
    public Button StartButton;

    public TextMeshProUGUI Page;
    public TextMeshProUGUI Description;
    
    public CanvasGroup CanvasGroup;
    
    public CanvasGroup PreviousCanvasGroup;
    public CanvasGroup NextCanvasGroup;
    
    public List<StageTutorial> StageTutorials;
  }
}