using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Settings.Mono
{
  public class SettingsView : MonoBehaviour
  {
    public RectTransform Background;

    public Button ButtonBack;
    public Button ButtonPrivacyPolicy;
    public Button ButtonRate;
    public Button ButtonGallery;

    public Button ButtonMusic;
    public Button ButtonSound;
    public Button ButtonHints;

    public Button Screen;

    public Transform SwitcherMusic;
    public Transform SwitcherSound;
    public Transform SwitcherHints;
  }
}