using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Animations.UI;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using AppacheAnalytics.Scripts.Interfaces;
using AppacheRemote.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Policy
{
  public class PrivacyPolicy : MonoBehaviour
  {
    [Header("AcceptScreen")] [SerializeField]
    private GameObject acceptScreen;

    [SerializeField] private Button linkButton;
    [SerializeField] private Button acceptButton1;

    [Header("ScrollScreen")] [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField] private GameObject scrollScreen;
    [SerializeField] private Button acceptButton2;
    [SerializeField] private Button backButton;

    [SerializeField] private Button denyButton;
    [SerializeField] private CanvasGroup denyButtonCanvasGroup;
    
    [SerializeField] private TextMeshProUGUI _policyText;

    public event Action ClosingStarted;
    public event Action ClosingFinished;

    private IAudioService _audioService;
    private IGameSaveSystem _gameSaveSystem;
    private IAnalytics _analytics;
    private IRemote _remote;

    private FadeImage _fadeBlurPanel;
    private List<FadeCanvasGroup> _fadeCanvasGroups;

    public void Construct(IGameSaveSystem gameSaveSystem, IAudioService audioService,
      ILocaleSystem localeSystem, IAnalytics analytics, IRemote remote)
    {
      _remote = remote;
      _analytics = analytics;
      _audioService = audioService;
      _gameSaveSystem = gameSaveSystem;

      AssignFades();

      gameObject.SetActive(false);

      denyButton.gameObject.SetActive(_gameSaveSystem.Get().IsPolicyAccepted);
      
      denyButtonCanvasGroup.interactable = false;
      denyButtonCanvasGroup.alpha = 0.5f;

      Localize(localeSystem);

      AssignAnimations();

      scrollRect.onValueChanged.AddListener(OnDrag);

      FetchPolicyText();
    }
    
    private void FetchPolicyText()
    {
      string policyText = _remote.GetPrivacyText();
      if (policyText.Equals("base") || String.IsNullOrEmpty(policyText))
        return;

      policyText = System.Text.RegularExpressions.Regex.Unescape(policyText);
      _policyText.text = policyText;
    }

    private void OnDrag(Vector2 arg0)
    {
      float normalizedPosition = scrollRect.verticalNormalizedPosition;

      if (normalizedPosition <= 0.01f)
      {
        denyButtonCanvasGroup.interactable = true;
        denyButtonCanvasGroup.alpha = 1f;
        
        new ButtonAnimation(denyButton.transform);
        new ButtonSound(_audioService, denyButton.gameObject, AudioId.SoundButton);
      }
    }

    public void OpenWindow()
    {
      gameObject.SetActive(true);

      if (_gameSaveSystem.Get().IsPolicyAccepted)
        ShowScrollScreen();
      else
        ShowAcceptScreen();

      acceptButton2.gameObject.SetActive(!_gameSaveSystem.Get().IsPolicyAccepted);
      backButton.gameObject.SetActive(_gameSaveSystem.Get().IsPolicyAccepted);

      AssignListeners();
      // _inputResolver.Disable();
      _fadeBlurPanel.Appear();
      _fadeCanvasGroups.ForEach(x => x.Appear());
    }

    public void Clear()
    {
      _fadeBlurPanel.Clear();
      _fadeCanvasGroups.ForEach(p => p.Clear());
    }

    private void AssignAnimations()
    {
      new ButtonAnimation(acceptButton1.transform);
      new ButtonAnimation(acceptButton2.transform);
      new ButtonAnimation(backButton.transform);

      new ButtonSound(_audioService, acceptButton1.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, acceptButton2.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, backButton.gameObject, AudioId.SoundButton);
      new ButtonSound(_audioService, linkButton.gameObject, AudioId.SoundButton);
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

    private void Accept()
    {
      _analytics.Send("btn_policy");

      _gameSaveSystem.Get().IsPolicyAccepted = true;
      _gameSaveSystem.Save();

      CloseWindow();
    }

    private void Deny()
    {
      _gameSaveSystem.Get().IsPolicyAccepted = false;
      _gameSaveSystem.Save();

      _audioService.Clean();

      SceneManager.LoadScene("Main");
    }

    private void Back() =>
      CloseWindow();

    private void CloseWindow()
    {
      _fadeBlurPanel.Disappear(null, OnClosingFinished);
      _fadeCanvasGroups.ForEach(x => x.Disappear());
      ClosingStarted?.Invoke();
      RemoveListeners();
      gameObject.SetActive(false);
      ClosingFinished?.Invoke();

      void OnClosingFinished()
      {
        gameObject.SetActive(false);
        ClosingFinished?.Invoke();
      }
    }

    private void ShowAcceptScreen()
    {
      acceptScreen.gameObject.SetActive(true);
      scrollScreen.gameObject.SetActive(false);
    }

    private void ShowScrollScreen()
    {
      acceptScreen.gameObject.SetActive(false);
      scrollScreen.gameObject.SetActive(true);
    }

    private void AssignListeners()
    {
      if (!_gameSaveSystem.Get().IsPolicyAccepted)
      {
        linkButton.onClick.AddListener(ShowScrollScreen);
        acceptButton1.onClick.AddListener(Accept);
        acceptButton2.onClick.AddListener(Accept);
      }
      else
      {
        denyButton.onClick.AddListener(Deny);
        backButton.onClick.AddListener(Back);
      }
    }

    private void RemoveListeners()
    {
      linkButton.onClick.RemoveListener(ShowScrollScreen);
      acceptButton1.onClick.RemoveListener(Accept);
      acceptButton2.onClick.RemoveListener(Accept);

      denyButton.onClick.RemoveListener(Deny);
      backButton.onClick.RemoveListener(Back);
    }

    private void AssignFades()
    {
      _fadeBlurPanel = new FadeImage(GetComponent<Image>());
      _fadeCanvasGroups = new List<FadeCanvasGroup>();

      var groups = GetComponentsInChildren<CanvasGroup>(true).ToList();
      groups.Remove(denyButtonCanvasGroup);
      
      foreach (CanvasGroup canvasGroup in groups)
        _fadeCanvasGroups.Add(new FadeCanvasGroup(canvasGroup));
    }
  }
}