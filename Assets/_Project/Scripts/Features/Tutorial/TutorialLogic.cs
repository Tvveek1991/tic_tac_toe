using _Project.Scripts.Features.Data;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Localization.Logic;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Features.Tutorial
{
  public class TutorialLogic
  {
    private const string TutorialPath = "Data/Tutorial/";

    private readonly TutorialFactory _tutorialFactory;
    private readonly TutorialData[] _tutorialData;
    
    private readonly ILocaleSystem _localeSystem;

    private int _activePage;
    
    public TutorialLogic(IAssetProvider assetProvider, ILocaleSystem localeSystem)
    {
      _localeSystem = localeSystem;
      _tutorialFactory = new TutorialFactory(assetProvider);
      _tutorialData = assetProvider.GetAllResources<TutorialData>(TutorialPath);
    }

    public void Construct()
    {
      _tutorialFactory.CreateTutorial();
      
      _tutorialFactory.TutorialView.BackButton.onClick.AddListener(() => SetActivity(false));
      
      _tutorialFactory.TutorialView.PreviousButton.onClick.AddListener(() => ChangePage(-1));
      _tutorialFactory.TutorialView.NextButton.onClick.AddListener(() => ChangePage(1));
      
      _tutorialFactory.TutorialView.StageTutorials.ForEach(item => item.Construct());
    }

    public void SetActivity(bool isOn)
    {
      _tutorialFactory.TutorialView.CanvasGroup.DOFade(isOn ? 1f : 0f, .25f);
      _tutorialFactory.TutorialView.CanvasGroup.blocksRaycasts = isOn;

      _activePage = 0;
      _tutorialFactory.TutorialView.PreviousButton.onClick.Invoke();
    }

    public void Exit() => 
      _tutorialFactory.Clear();

    private void ChangePage(int direction)
    {
      _activePage += direction;
      _activePage = Mathf.Clamp(_activePage, 0, _tutorialData.Length - 1);

      _tutorialFactory.TutorialView.Page.text = (_activePage + 1) + " / " + _tutorialData.Length;
      _tutorialFactory.TutorialView.Description.text = _localeSystem.GetText(_tutorialData[_activePage].DescriptionKey);
      
      _tutorialFactory.TutorialView.StageTutorials.ForEach(item => item.Hide());
      _tutorialFactory.TutorialView.StageTutorials[_activePage].Show();
      
      FadeSlideButtons();
    }

    private void FadeSlideButtons()
    {
      _tutorialFactory.TutorialView.PreviousCanvasGroup.DOFade(_activePage == 0 ? .5f : 1f, .25f);
      _tutorialFactory.TutorialView.NextCanvasGroup.DOFade(_activePage == _tutorialData.Length - 1 ? .5f : 1f, .25f);
    }
  }
}