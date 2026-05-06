using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.DisableAds.Mono
{
  public class DisableAdsWindow : MonoBehaviour
  {
    public Button ShowBtn;
    public Button CloseBtn;
    public Button ContinueBtn;
    public Button PurchaseBtn;

    public SpriteSwap SwapperBackIcon;
    
    public Text PurchasePrice;
    
    public TextMeshProUGUI AdsWatchedProgress;
    public TextMeshProUGUI[] DisableAdsTimers;

    public GameObject InfoPanelBefore;
    public Transform InfoPanelAfter;
    public GameObject AdsOfferEnabled;
    public GameObject AdsOfferDisabled;
  }
}