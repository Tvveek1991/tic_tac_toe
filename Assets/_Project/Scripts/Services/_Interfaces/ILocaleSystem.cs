using _Project.Scripts.Service_Base;
using UnityEngine;

namespace _Project.Scripts.Services.Localization.Logic
{
  public interface ILocaleSystem : IService
  {
    void SetLanguage(SystemLanguage language);
    string GetText(string key);
    string GetKey(string key);
  }
}