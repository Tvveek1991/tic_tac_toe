using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.HomeScreen.Mono
{
  public class HomeScreen : MonoBehaviour
  {
    public Button ShopButton;
    public Button GalleryButton;
    public Button SettingsButton;
    public Button SecretaryButton;
    public Button GamesButton;
    public Button StoriesButton;
    public Button IapShopButton;
    public Button NoAdsButton;

    public void Construct(ILocaleSystem localeSystem, Action toGallery, Action toSecretary, Action openSettingsWindow,
      Action toShop, Action toGames, Action toStories, Action toIapShop, Action toNoAds)
    {
      ShopButton.onClick.AddListener(toShop.Invoke);
      GamesButton.onClick.AddListener(toGames.Invoke);
      GalleryButton.onClick.AddListener(toGallery.Invoke);
      StoriesButton.onClick.AddListener(toStories.Invoke);
      IapShopButton.onClick.AddListener(toIapShop.Invoke);
      SettingsButton.onClick.AddListener(openSettingsWindow.Invoke);
      SecretaryButton.onClick.AddListener(toSecretary.Invoke);
      NoAdsButton.onClick.AddListener(toNoAds.Invoke);

      CreateAnimations();

      Localize(localeSystem);
    }

    private void Localize(ILocaleSystem localeSystem)
    {
      List<LocalizedMainText> localizedTexts = transform.GetComponentsInChildren<LocalizedMainText>(true).ToList();
      localizedTexts.ForEach(item =>
      {
        item.Construct(localeSystem);
        item.Localize();
      });
    }

    public void Clear() { }

    private void CreateAnimations()
    {
      new ButtonAnimation(ShopButton.transform);
      new ButtonAnimation(GamesButton.transform);
      new ButtonAnimation(GalleryButton.transform);
      new ButtonAnimation(StoriesButton.transform);
      new ButtonAnimation(IapShopButton.transform);
      new ButtonAnimation(SettingsButton.transform);
      new ButtonAnimation(SecretaryButton.transform);
      new ButtonAnimation(NoAdsButton.transform);
    }
  }
}