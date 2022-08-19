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
  public class HomeView : MonoBehaviour
  {
    public Button ShopButton;
    public Button ModeButton;
    public Button StartButton;
    public Button NoAdsButton;
    public Button IapShopButton;
    public Button SettingsButton;

    public void Construct(ILocaleSystem localeSystem, Action openSettingsWindow, Action toShop, Action toGame, Action toGameMode,
      Action toIapShop, Action toNoAds)
    {
      ShopButton.onClick.AddListener(toShop.Invoke);
      NoAdsButton.onClick.AddListener(toNoAds.Invoke);
      StartButton.onClick.AddListener(toGame.Invoke);
      IapShopButton.onClick.AddListener(toIapShop.Invoke);
      ModeButton.onClick.AddListener(toGameMode.Invoke);
      SettingsButton.onClick.AddListener(openSettingsWindow.Invoke);

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

    private void CreateAnimations()
    {
      new ButtonAnimation(ShopButton.transform);
      new ButtonAnimation(StartButton.transform);
      new ButtonAnimation(ModeButton.transform);
      new ButtonAnimation(IapShopButton.transform);
      new ButtonAnimation(SettingsButton.transform);
      new ButtonAnimation(NoAdsButton.transform);
    }
  }
}