using _Project.Scripts.Features.ShopScreen.Serialized;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  public class GameCustomData : BaseContentData
  {
    [SerializeField] private ItemType _itemType;

    public ItemType ItemType => _itemType;
  }
}