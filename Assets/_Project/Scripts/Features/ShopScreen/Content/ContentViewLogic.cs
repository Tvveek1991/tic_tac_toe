using System.Collections.Generic;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ShopScreen.Mono;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class ContentViewLogic : IContentViewLogic
  {
    private readonly IAudioService _audioService;
    private readonly IGameMetaProvider _gameMetaProvider;

    private readonly List<IconPreview> _shopViewPreview;

    public ContentViewLogic(IGameMetaProvider gameMetaProvider, IAudioService audioService, List<IconPreview> shopViewPreview)
    {
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;

      _shopViewPreview = shopViewPreview;
    }

    public void LoadContent(ShopItemData shopItemData)
    {
      if (shopItemData.ItemType == ItemType.Music)
        _audioService.Play(_gameMetaProvider.GetMusic());

      LoadPreview(shopItemData);
    }

    private void LoadPreview(ShopItemData itemData)
    {
      for (var i = 0; i < _shopViewPreview.Count; i++)
      {
        int i1 = i;
        _shopViewPreview[i].Hide(() =>
        {
          if (i1 >= itemData.PreviewMeshes.Count) 
            return;
          
          _shopViewPreview[i1].SetVisual(itemData.PreviewMeshes[i1], itemData.PreviewMaterials[i1]);
          _shopViewPreview[i1].Show(i1 == 0 ? itemData.PreviewScaleFactor : 1);
        });
      }
    }
  }
}