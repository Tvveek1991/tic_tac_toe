using System;
using System.Collections.Generic;
using System.Linq;
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

    private const float AnimDuration = .5f;
    private const float WindowAnimStepY = 1000f;

    public Button BackButton;

    public Transform ItemViewsParent;

    public CanvasGroup TopPanel;
    public CanvasGroup MainPanel;

    public SectionButton[] SectionButtons;

    [Space(20)] 
    [SerializeField] private List<IconPreview> _preview;

    private Vector3 _windowDefaultPos;
    private Action _onClickBack;

    public void Construct(Action onClickBack)
    {
      _onClickBack = onClickBack;
      BackButton.onClick.AddListener(Hide);
      
      _preview.ForEach(item => item.Construct());

      _windowDefaultPos = MainPanel.GetComponent<RectTransform>().anchoredPosition;
      Show();
    }

    public ScrollViewSectionButton GetSectionButton(ItemType type) =>
      SectionButtons.FirstOrDefault(x => x.Type == type)?.Button;

    public List<IconPreview> GetIconPreview() =>
      _preview;
    
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
      
      _preview.ForEach(item => item.Hide(null));

      MainPanel.interactable = false;
      var mainRect = MainPanel.GetComponent<RectTransform>();
      mainRect.anchoredPosition = _windowDefaultPos;   
      
      mainRect.DOAnchorPos(_windowDefaultPos - Vector3.up * WindowAnimStepY, AnimDuration)
        .OnComplete(delegate
        {
          MainPanel.interactable = true;
          _onClickBack?.Invoke();
        });
    }
  }
}