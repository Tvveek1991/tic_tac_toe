using System;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Features.ScrollView;
using _Project.Scripts.Features.ShopScreen.Serialized;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.ShopScreen.Mono
{
  public class ShopView : MonoBehaviour
  {
    [Serializable]
    public class SectionButton
    {
      public ItemType Type;
      public ScrollViewSectionButton Button;
    }

    private const float ClearDuration = .5f;
    private const float AnimDuration = .5f;
    private const float WindowAnimStepY = 1000f;

    public Button BackButton;

    public Image ProgressInfo;
    public TextMeshProUGUI ProgressText;

    public Transform ShopParent;
    public Transform ItemViewsParent;
    public Transform SkinEffectParent;

    public CanvasGroup TopPanel;
    public CanvasGroup MainPanel;

    public SectionButton[] SectionButtons;

    private Vector3 _windowDefaultPos;
    private Action _onClickBack;

    public void Construct(Action onClickBack)
    {
      _onClickBack = onClickBack;
      BackButton.onClick.AddListener(Hide);

      _windowDefaultPos = MainPanel.GetComponent<RectTransform>().anchoredPosition;
      Show();
    }

    public void AddProgress(int progress, int max)
    {
      float percent = (float)progress / max;
      ProgressInfo.DOFillAmount(percent, 0.2f);
      ProgressText.text = progress + "/" + max;
    }

    public void ClearPassiveProgress() =>
      ProgressInfo.DOFillAmount(0f, ClearDuration * ProgressInfo.fillAmount);


    public ScrollViewSectionButton GetSectionButton(ItemType type) =>
      SectionButtons.FirstOrDefault(x => x.Type == type)?.Button;

    private void Show()
    {
      TopPanel.interactable = false;
      TopPanel.alpha = 0;
      TopPanel.DOFade(1, AnimDuration);

      MainPanel.interactable = false;
      var mainRect = MainPanel.GetComponent<RectTransform>();
      mainRect.anchoredPosition = _windowDefaultPos - Vector3.up * WindowAnimStepY;
      mainRect.DOAnchorPos(_windowDefaultPos, AnimDuration)
        .OnComplete(delegate
        {
          TopPanel.interactable = true;
          MainPanel.interactable = true;
        });
    }

    private void Hide()
    {
      if (!MainPanel.interactable)
        return;
      
      TopPanel.alpha = 1;
      TopPanel.DOFade(0, AnimDuration);

      MainPanel.interactable = false;
      RectTransform mainRect = MainPanel.GetComponent<RectTransform>();
      mainRect.anchoredPosition = _windowDefaultPos;   
      
      mainRect.DOAnchorPos(_windowDefaultPos - Vector3.up * WindowAnimStepY, AnimDuration)
        .OnComplete(delegate
        {
          // TopPanel.interactable = true;
          MainPanel.interactable = true;
          _onClickBack?.Invoke();
        });
    }
  }
}