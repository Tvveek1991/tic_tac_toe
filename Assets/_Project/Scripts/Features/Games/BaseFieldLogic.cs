using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Games
{
  public abstract class BaseFieldLogic
  {
    protected Transform FieldParent;
    
    protected IAudioService AudioService;
    protected IAssetProvider AssetProvider;
    
    public BaseFieldLogic(IAssetProvider assetProvider, IAudioService audioService, Transform fieldParent)
    {
      AudioService = audioService;
      AssetProvider = assetProvider;
    }

    public virtual void Construct()
    {
      Debug.Log("Base");
    }

    public virtual void CalculateField(int height = 3, int width = 3)
    {
      
    }
  }
}