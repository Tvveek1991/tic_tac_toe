using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.NotLoadedWindow
{
  public class NotLoadedViewLogic : INotLoadedViewLogic
  {
    private NotLoadedMono _notLoadedMono;

    private readonly IAssetProvider _assetProvider;
    private readonly ILocaleSystem _localeSystem;
    
    private INotLoadedFactory _notLoadedFactory;

    public NotLoadedViewLogic(IAssetProvider assetProvider, ILocaleSystem localeSystem)
    {
      _localeSystem = localeSystem;
      _assetProvider = assetProvider;
    }

    public void Open()
    {    
      Initialize(_assetProvider, _localeSystem);

      _notLoadedMono.gameObject.SetActive(true);
      
      _notLoadedMono.Background.alpha = 0;
      _notLoadedMono.Main.localScale = 0.7f * Vector3.one;
      
      _notLoadedMono.Background.DOFade(1, 0.5f);
      _notLoadedMono.Main.DOScale(1, 0.5f);
    }

    private void Close()
    {
      _notLoadedMono.Background.alpha = 1;
      _notLoadedMono.Main.localScale = Vector3.one;

      _notLoadedMono.Background.DOFade(0, 0.5f);
      _notLoadedMono.Main.DOScale(0.2f, 0.5f).onComplete += OnComplete;
    }

    private void Initialize(IAssetProvider assetProvider, ILocaleSystem localeSystem)
    {
      _notLoadedFactory = new NotLoadedFactory(assetProvider);

      _notLoadedFactory.CreateScreen();
      _notLoadedMono = _notLoadedFactory.NotLoadedMono;

      _notLoadedMono.OkBtn.onClick.AddListener(Close);

      Localize(localeSystem);
    }

    private void OnComplete()
    {
      _notLoadedMono.gameObject.SetActive(false);
      _notLoadedFactory.ClearScreen();
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = _notLoadedMono.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }
  }
}