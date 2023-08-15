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

    public TextMeshProUGUI Page;
    public TextMeshProUGUI Description;
    
    public CanvasGroup CanvasGroup;
  }
}