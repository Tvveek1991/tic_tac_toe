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
    public float PreviewScaleFactor => _previewScaleFactor;
    public List<Mesh> PreviewMeshes => _previewMeshes;
    public List<Material> PreviewMaterials => _previewMaterials;

    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite _icon;
    
    [SerializeField] private float _previewScaleFactor = 1f;
    [SerializeField] private List<Mesh> _previewMeshes;
    [SerializeField] private List<Material> _previewMaterials;
  }
}