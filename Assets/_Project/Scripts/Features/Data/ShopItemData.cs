using System.Collections.Generic;
using _Project.Scripts.Features.ShopScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Upgrade Item", menuName = "Data/Upgrade Data", order = 0)]
  public class ShopItemData : BaseContentData
  {
    public ItemType ItemType => itemType;
    public Sprite Icon => _icon;

    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite _icon;
  }
}