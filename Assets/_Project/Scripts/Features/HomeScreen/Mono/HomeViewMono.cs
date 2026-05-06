using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.HomeScreen.Mono
{
  public class HomeViewMono : MonoBehaviour
  {
    public Button ClassicButton;
    public Button BuhurtButton;
    public Button ShopButton;
    public Button NoAdsButton;
    public Button SettingsButton;
    
    [Header("Enemy type"), Space(20)]
    public Transform SingleIcon;
    public Transform FriendsIcon;
    public Button SingleButton;
    public Button FriendsButton;
    public TextMeshProUGUI SingleText;
    public TextMeshProUGUI FriendsText;
  }
}