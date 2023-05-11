using _Project.Scripts.Features.Customization.Interface;
using UnityEngine;

namespace _Project.Scripts.Features.Customization.Serialized
{
  [CreateAssetMenu(fileName = "Skins Config", menuName = "Skins/Skin Data", order = 0)]
  public class SkinDataConfig : ScriptableObject, ISkinDataAction
  {
    public string Guid
    {
      get => _guid;
      set => _guid = value;
    }

    [SerializeField] private string _guid = string.Empty;
    private string SaveKey => "SkinData-" + Guid;
    private string SaveKeyActive => "SkinData-" + Guid + "-SaveKeyActive";

    [SerializeField] private SkinType _skinType;
    [SerializeField] private PriceType _priceType;
    [SerializeField] [Range(0, 1000)] private int price;
    [SerializeField] [Range(0, 1000)] private int testPrice;
    [SerializeField] private GameObject _icon;
    [SerializeField] private string _header;

    public SkinType GetSkinType() =>
      _skinType;

    public PriceType GetPriceType() =>
      _priceType;

    public int GetPrice() =>
      price;

    public int GetTestPrice() =>
      testPrice;

    public GameObject GetIcon() =>
      _icon;

    public string GetHeader() =>
      _header;

    public bool IsReceived() =>
      PlayerPrefs.GetInt(SaveKey, 0) != 0;

    public void Receive() =>
      PlayerPrefs.SetInt(SaveKey, 1);

    public string GetGuid() =>
      Guid;

    public void SetActive(bool value) =>
      PlayerPrefs.SetInt(SaveKeyActive, value ? 1 : 0);

    bool ISkinData.IsActivated() =>
      PlayerPrefs.GetInt(SaveKeyActive) != 0;
  }
}