using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.ShopScreen.Serialized;
using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class LookDisplayContent : BaseDisplayContent
  {
    private readonly IAudioService _audioService;
    private readonly IGameSaveSystem _gameSaveSystem;
    private readonly IGameMetaProvider _gameMetaProvider;

    public LookDisplayContent(IGameMetaProvider gameMetaProvider, IGameSaveSystem gameSaveSystem, IAudioService audioService)
    {
      _gameMetaProvider = gameMetaProvider;
      _audioService = audioService;
      _gameSaveSystem = gameSaveSystem;
    }
    
    public override void Display(ShopItemData shopItemData)
    {
      
      
      if(shopItemData.ItemType == ItemType.Music)
        _audioService.Play(_gameMetaProvider.GetMusic());
    }
  }
}