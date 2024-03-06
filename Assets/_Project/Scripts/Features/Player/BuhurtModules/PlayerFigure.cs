using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.DragAndDrop.Interface;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Player.BuhurtModules
{
  public class PlayerFigure : MonoBehaviour, IPointerDownHandler
  {
    private const string ActiveCellPath = "activeCell";
    private const string InactiveCellPath = "inactiveCell";
    
    private readonly int _color = Shader.PropertyToID("_Color");
    private readonly int _mainTex = Shader.PropertyToID("_MainTex");

    [SerializeField] private SymbolSize _size;

    private Image _frame;
    private BoxCollider _boxCollider;

    private MeshFilter _mesh;
    private MeshRenderer _meshRenderer;
    
    private MeshFilter _shadowMesh;
    private MeshRenderer _shadowRenderer;
    
    private MeshRenderer _whiteBack;

    private DragableData _dragableData;
    private DragablePart _dragablePart;

    private IDragDrop _dragDrop;

    private IInputResolver _inputResolver;

    private bool _isLock;
    private bool _isBackOn;

    private Material _activeFigure;
    private Material _inactiveFigure;

    private Sprite _activeCell;
    private Sprite _inactiveCell;

    public SymbolSign Sign { get; private set; }
    public SymbolSize Size => _size;

    private Action _hidePotentials;
    private Action<DragablePart> _showPotentialTurn;

    public void Construct(IDragDrop dragDrop, IInputResolver inputResolver, IAssetProvider assetProvider,
      Action<DragablePart> showPotentialTurn, Action hidePotentials, bool isBackOn) 
    {
      _mesh = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();
      
      _shadowMesh = GetComponentsInChildren<MeshFilter>()[1];
      _shadowRenderer = GetComponentsInChildren<MeshRenderer>()[1];
      
      _whiteBack = GetComponentsInChildren<MeshRenderer>()[2];

      _frame = GetComponent<Image>();
      _boxCollider = GetComponentInChildren<BoxCollider>();

      _hidePotentials = hidePotentials;
      _showPotentialTurn = showPotentialTurn;

      _isBackOn = isBackOn;
      
      _dragDrop = dragDrop;
      _inputResolver = inputResolver;

      _dragablePart = CreateDragableFromGameIconPrefab();

      _activeCell = assetProvider.GetResource<Sprite>(ActiveCellPath);
      _inactiveCell = assetProvider.GetResource<Sprite>(InactiveCellPath);
    }

    public void FirstLoadFigure(SymbolSign symbolSign, Mesh mesh, Material material, Vector3 angle)
    {
      PrepareMaterials(material);

      Sign = symbolSign;

      _mesh.mesh = mesh;
      _mesh.transform.localRotation = Quaternion.Euler(angle);
      
      _shadowMesh.mesh = mesh;
    }

    public void SetVisibility(bool isOn)
    {
      if (_size == SymbolSize.Default)
      {
        _frame.enabled = false;
        _meshRenderer.enabled = false;
        return;
      }

      _isLock = !isOn;
      _frame.sprite = isOn ? _activeCell : _inactiveCell;
      _meshRenderer.material = isOn ? _activeFigure : _inactiveFigure;
    }

    public void SetSizeToMoved() =>
      _size = SymbolSize.Default;

    public void OnPointerDown(PointerEventData eventData)
    {
      if (_dragDrop == null)
        return;

      if (_size == SymbolSize.Default || _inputResolver.IsLocked)
        return;

      if (_isLock)
        return;

      _dragablePart.SetDragableData(new DragableData(Sign, _size));
      _dragDrop.BindPartToCursor(_dragablePart);
    }

    private void PrepareMaterials(Material material)
    {
      var inactive = new Material(_meshRenderer.material);
      var color = new Color32(180, 180, 180, 255); /*material.GetColor(_color);*/
      // color.a = .4f;
      inactive.SetColor(_color, color);
      inactive.SetTexture(_mainTex, material.GetTexture(_mainTex));

      _inactiveFigure = inactive;
      _activeFigure = new Material(material);
    }

    private DragablePart CreateDragableFromGameIconPrefab()
    {
      var dragablePart = _mesh.gameObject.AddComponent<DragablePart>();
      dragablePart.Construct(OnStartDrag, OnPin, OnReturn);

      return dragablePart;
    }

    private void OnStartDrag()
    {
      _frame.enabled = false;
      _shadowRenderer.enabled = _isBackOn;
      _whiteBack.enabled = _isBackOn;
      _boxCollider.enabled = false;
      _showPotentialTurn.Invoke(_dragablePart);
    }

    private void OnPin() { }

    private void OnReturn()
    {
      _whiteBack.enabled = false;
      _shadowRenderer.enabled = false;
      _frame.enabled = _size != SymbolSize.Default;
      _boxCollider.enabled = true;
      _hidePotentials?.Invoke();
    }
  }
}