using _Project.Scripts.Features.Settings.Mono;
using _Project.Scripts.Services.Money.Mono;

namespace _Project.Scripts.Features
{
  public interface IUiFactory
  {
    SettingsView SettingsView { get; }
    NotLoaded.NotLoadedWindow NoInternetWindow { get; }

    void CreateSettingsView();
    void SpawnNoInternetConnection();
    void ClearSettingsWindow();
    void ClearNoInternetWindow();
  }
}