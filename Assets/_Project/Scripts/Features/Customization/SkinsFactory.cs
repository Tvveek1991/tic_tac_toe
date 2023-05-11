using _Project.Scripts.Features.Customization.Mono;
using _Project.Scripts.Features.Customization.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Customization
{
  public class SkinsFactory : ISkinsFactory
  {
    private const string SkinsConfigPath = "Skins Config";
    private const string SkinsWindowPrefabPath = "Prefabs/GUI/Skins Window";
    private const string ApproveWindowPath = "Prefabs/GUI/Approve Window";
    private const string ScrollViewPrefabPath = "Prefabs/GUI/Skins Page Scroll View";
    private const string GUISkinPanelPath = "Prefabs/GUI/Skin Panel";
    private const string Canvas = "Canvas";
    private const string SkinsViewParent = "Shop Window";
    private const string CanvasWindows = "WindowsUI";

    private readonly IAssetProvider _assetProvider;

    private readonly Transform _parent;
    private readonly Transform _parentWindows;
    private readonly SkinsView.Factory _skinsViewFactory;

    private SkinsView _skinsView;
    private ApproveWindow _approveWindow;
    private GameObject _scrollView;
    private SkinPanel _skinPanel;
    
    public SkinsFactory(IAssetProvider assetProvider, SkinsView.Factory skinsViewFactory)
    {
      _assetProvider = assetProvider;
      _skinsViewFactory = skinsViewFactory;
      _parent = GameObject.Find(Canvas).transform;
      _parentWindows = GameObject.Find(CanvasWindows).transform;
    }

    public SkinsConfig GetSkinsConfig() =>
      _assetProvider.GetResource<SkinsConfig>(SkinsConfigPath);

    public SkinsView CreateSkinsView()
    {
      _skinsView = _skinsViewFactory.Create(_assetProvider.GetResource<SkinsView>(SkinsWindowPrefabPath), GameObject.Find(SkinsViewParent).transform);
      return _skinsView;
    }

    public ApproveWindow CreateApproveWindow(Transform parent)
    {
     _approveWindow = Object.Instantiate(_assetProvider.GetResource<ApproveWindow>(ApproveWindowPath), _parentWindows);
     return _approveWindow;
    }

    public GameObject CreateScrollView(Transform parent)
    {
      _scrollView = Object.Instantiate(_assetProvider.GetResource<GameObject>(ScrollViewPrefabPath), parent);
      return _scrollView;
    }

    public SkinPanel CreateSkinPanel(Transform parent)
    {
      _skinPanel = Object.Instantiate(_assetProvider.GetResource<SkinPanel>(GUISkinPanelPath), parent);
      return _skinPanel;
    }

    public void Clear()
    {
      Object.Destroy(_skinsView.gameObject);
      Object.Destroy(_approveWindow.gameObject);
      Object.Destroy(_scrollView);
      Object.Destroy(_skinPanel.gameObject);
    }
  }
}