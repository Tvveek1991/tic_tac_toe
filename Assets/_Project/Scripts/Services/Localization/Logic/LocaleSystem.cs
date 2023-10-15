using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using Newtonsoft.Json;

namespace _Project.Scripts.Services.Localization.Logic
{
  public class LocaleSystem : ILocaleSystem
  {
    private const string ActiveLanguageKey = "ActiveLanguageKey";
    
    private const string PathMain = "Main";

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _dictionary;

    private string ActiveLanguage
    {
      get => PlayerPrefs.GetString(ActiveLanguageKey, "");
      set => PlayerPrefs.SetString(ActiveLanguageKey, value);
    }
      
    public LocaleSystem(IAssetProvider assetProvider) => 
      Initialize(assetProvider);

    private void Initialize(IAssetProvider assetProvider)
    {
      string jsonMain = assetProvider.GetResource<TextAsset>(PathMain).text;

      _dictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
      {
        {PathMain, JsonToDictionary(jsonMain)},
      };

      FetchLanguage(Application.systemLanguage);
    }

    public void SetLanguage(SystemLanguage language) => 
      FetchLanguage(language);

    public string GetText(string key)
    {
      _dictionary[PathMain][ActiveLanguage].TryGetValue(key, out  string text);
      return text;
    }
    
    public string GetKey(string text) => 
      _dictionary.ContainsKey(PathMain) ? _dictionary[PathMain]["ru"].First(x => x.Value == text).Key : "";

    private Dictionary<string, Dictionary<string, string>> JsonToDictionary(string json) => 
      JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

    private void FetchLanguage(SystemLanguage language)
    {
      ActiveLanguage = language switch
      {
        SystemLanguage.English => "en",
        SystemLanguage.Russian => "ru",
        SystemLanguage.Spanish => "es",
        SystemLanguage.Portuguese => "pt",
        _ => "en"
      };
    }
  }
}