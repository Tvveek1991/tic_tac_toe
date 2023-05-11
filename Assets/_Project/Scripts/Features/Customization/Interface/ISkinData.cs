using _Project.Scripts.Features.Customization.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Customization.Interface
{
  public interface ISkinData
  {
    SkinType GetSkinType();
    PriceType GetPriceType();
    int GetPrice();
    int GetTestPrice();
    GameObject GetIcon();
    string GetHeader();
    bool IsReceived();
    string GetGuid();
    void SetActive(bool value);
    bool IsActivated();
  }

  public interface ISkinDataAction : ISkinData
  {
    void Receive();
  }
}