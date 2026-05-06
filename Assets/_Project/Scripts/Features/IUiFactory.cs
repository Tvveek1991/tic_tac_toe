using _Project.Scripts.Features.Settings.Mono;

namespace _Project.Scripts.Features
{
  public interface IUiFactory
  {
    SettingsView SettingsView { get; }
    NotLoaded.NotLoadedWindow NoInternetView { get; }

    void CreateSettingsView();
    void SpawnNoInternetConnectionView();
    void ClearSettingsView();
    void ClearNoInternetView();
  }
}