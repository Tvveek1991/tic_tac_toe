using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Animation;
using _Project.Scripts.Animations.UI;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using _Project.Scripts.Services.Audio.Executor;
using _Project.Scripts.Services.Localization.Logic;
using _Project.Scripts.Services.Localization.Mono;
using DG.Tweening;
using SableGamesAnalytics.Scripts.Interfaces;
using SableGamesRemote.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Policy
{
  public class PrivacyPolicy : MonoBehaviour
  {
    private const float Duration = .25f;

    [SerializeField] private CanvasGroup scrollCanvasGroup;
    [SerializeField] private CanvasGroup deniedCanvasGroup;

    [Header("Accept part")] [SerializeField]
    private Button buttonLink;

    [SerializeField] private Button buttonAccept;

    [Header("Scroll part")] [SerializeField]
    private Button buttonBack;

    [SerializeField] private Button buttonScrollAccept;
    [SerializeField] private GameObject toggles;

    [SerializeField] private Toggle toggleDeny;
    [SerializeField] private Toggle togglePersonalized;

    [SerializeField] private TextMeshProUGUI _policyText;

    [Header("Denied part")] [SerializeField]
    private Button buttonYes;

    [SerializeField] private Button buttonNo;

    public event Action ClosingStarted;
    public event Action ClosingFinished;

    private IAudioService _audioService;
    private IGameSaveSystem _gameSaveSystem;
    private IAnalytics _analytics;
    private IRemote _remote;

    private FadeImage _fadeBlurPanel;
    private List<FadeCanvasGroup> _fadeCanvasGroups;

    private Action _onDeny;

    public void Construct(IGameSaveSystem gameSaveSystem, IAudioService audioService,
      IAnalytics analytics, IRemote remote, Action onDeny = null)
    {
      _onDeny = onDeny;
      
      _remote = remote;
      _analytics = analytics;
      _audioService = audioService;
      _gameSaveSystem = gameSaveSystem;
      
      AddSounds();
      AssignFades();
      AssignAnimations();
      AssignListeners();
      
      VisibilityScrollPart(_gameSaveSystem.Get().IsPolicyAccepted);

      toggles.gameObject.SetActive(_gameSaveSystem.Get().IsPolicyAccepted);
      buttonBack.gameObject.SetActive(_gameSaveSystem.Get().IsPolicyAccepted);
      buttonScrollAccept.gameObject.SetActive(!_gameSaveSystem.Get().IsPolicyAccepted);
      
      FetchPolicyText();
      
      gameObject.SetActive(false);
    }

    private void FetchPolicyText()
    {
      string policyText = _remote.GetPrivacyText();
      if (policyText.Equals("base") || String.IsNullOrEmpty(policyText))
        return;

      policyText = System.Text.RegularExpressions.Regex.Unescape(policyText);
      _policyText.text = policyText;
    }

    public void OpenWindow()
    {
      gameObject.SetActive(true);


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
      new ButtonAnimation(buttonScrollAccept.transform);
      new ButtonAnimation(buttonAccept.transform);
      new ButtonAnimation(buttonBack.transform);
      new ButtonAnimation(buttonYes.transform);
      new ButtonAnimation(buttonNo.transform);
    }

    private void AddSounds()
    {
      new ButtonSound(_audioService, buttonScrollAccept.gameObject, AudioId.Click);
      new ButtonSound(_audioService, buttonBack.gameObject, AudioId.ClickBack);
      new ButtonSound(_audioService, buttonAccept.gameObject, AudioId.Click);
      new ButtonSound(_audioService, buttonLink.gameObject, AudioId.Click);
      new ButtonSound(_audioService, buttonYes.gameObject, AudioId.Click);
      new ButtonSound(_audioService, buttonNo.gameObject, AudioId.Click);
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

      _onDeny?.Invoke();
    }

    private void Back() =>
      CloseWindow();

    private void CloseWindow()
    {
      _fadeBlurPanel.Disappear(null, OnClosingFinished);
      _fadeCanvasGroups.ForEach(x => x.Disappear());
      ClosingStarted?.Invoke();

      gameObject.SetActive(false);
      ClosingFinished?.Invoke();

      void OnClosingFinished()
      {
        gameObject.SetActive(false);
        ClosingFinished?.Invoke();
      }
    }

    private void VisibilityScrollPart(bool isVisible)
    {
      scrollCanvasGroup.DOFade(isVisible ? 1f : 0f, Duration);
      scrollCanvasGroup.blocksRaycasts = isVisible;
    }

    private void VisibilityDeniedPart(bool isVisible)
    {
      deniedCanvasGroup.DOFade(isVisible ? 1f : 0f, Duration);
      deniedCanvasGroup.blocksRaycasts = isVisible;
    }

    private void AssignListeners()
    {
      buttonBack.onClick.AddListener(Back);
      buttonAccept.onClick.AddListener(Accept);
      buttonScrollAccept.onClick.AddListener(Accept);
      buttonLink.onClick.AddListener(() => VisibilityScrollPart(true));
      
      buttonYes.onClick.AddListener(Deny);
      buttonNo.onClick.AddListener(() =>
      {
        toggleDeny.isOn = true;
        VisibilityDeniedPart(false);
      });

      toggleDeny.onValueChanged.AddListener(delegate(bool isOn)
      {
        if(!isOn)
          VisibilityDeniedPart(true);
      });
    }

    private void AssignFades()
    {
      _fadeBlurPanel = new FadeImage(GetComponent<Image>());
      _fadeCanvasGroups = new List<FadeCanvasGroup>();

      List<CanvasGroup> groups = GetComponentsInChildren<CanvasGroup>(true).ToList();
      groups.Remove(scrollCanvasGroup);
      groups.Remove(deniedCanvasGroup);

      foreach (CanvasGroup canvasGroup in groups)
        _fadeCanvasGroups.Add(new FadeCanvasGroup(canvasGroup));
    }
  }
}