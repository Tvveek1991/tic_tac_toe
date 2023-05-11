using System;
using _Project.Scripts.Features.Customization.Interface;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Customization.Mono
{
  public class SkinPanel : MonoBehaviour
  {
    private const float Delay = .25f;
    
    public GameObject Lock;
    public Transform Icon;
    public Image CheckIconObject;
    public Button Button;
    public GameObject ParticleSystem;
    public Text ComingSoon;
    //
    // private SkinsRender _skinsRender;
  
    private ISkinData _skinData;

    public void Initialize(ISkinData skinData, SkinsPage page, Action<SkinsPage, SkinPanel> onClick)
    {
      _skinData = skinData;
    
      name = name.Replace("(Clone)", "");
    
      //Price.text = _skinData.GetPrice().ToString();
      //Header.text = _skinData.GetHeader();
      Lock.GetComponent<SpriteRenderer>().color = Color.white;
      ComingSoon.enabled = false;
      // SpriteState.Enable();

      /*_skinsRender = Instantiate(_skinData.GetIcon(), Icon).GetComponent<SkinsRender>();
      if(!_skinData.IsReceived())
        _skinsRender.Close();*/

      Button.onClick.AddListener(delegate { onClick(page, this); });
    }
    
    public void Initialize(ILocaleSystem localeSystem) =>
      ComingSoon.text = localeSystem.GetText("ComingSoon");

    public ISkinData GetSkinData() =>
      _skinData;
  
    public void Activate()
    {
      CheckIconObject.gameObject.SetActive(true);
      _skinData.SetActive(true);
    }

    public void Deactivate()
    {
      CheckIconObject.gameObject.SetActive(false);
      _skinData.SetActive(false);
    }

    public void ChangeStatusToReceive()
    {
      Lock.SetActive(false);
      // _skinsRender.Open();

      if(!gameObject.activeInHierarchy)
        return;
      
      CheckIconObject.DOColor(Color.white, Delay)
        .SetLoops(4, LoopType.Yoyo)
        .SetEase(Ease.Linear);

      /*_skinsRender.transform.DOScale(_skinsRender.transform.localScale * 1.2f, Delay)
        .SetLoops(4, LoopType.Yoyo)
        .SetEase(Ease.Linear);*/
      
      ParticleSystem.SetActive(true);
      //Header.gameObject.SetActive(true);
    }
  }
}