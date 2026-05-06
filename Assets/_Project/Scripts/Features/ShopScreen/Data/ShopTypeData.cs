using _Project.Scripts.Features.ShopScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen.Data
{
  [CreateAssetMenu(fileName = "Shop Type", menuName = "ScriptableObjects/Shop/Shop Type Data", order = 0)]
  public class ShopTypeData : ScriptableObject
  {
    public ItemType Type => _type;
    public string NameKey => _nameKey;

    public Sprite Icon_on;
    public Sprite Icon_off;
    
    [SerializeField] private ItemType _type;
    [SerializeField] private string _nameKey;
  }
}