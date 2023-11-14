using System;
using _Project.Scripts.Features.Data;
using _Project.Scripts.Features.GameScreen.Serialized;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Enum;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Field.Mono
{
  public class SymbolCell : MonoBehaviour
  {
    private const float Duration = .5f;
    private const float DurationPotentialTurn = .25f;

    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    
    [SerializeField] private Transform _potentialTurn;
    
    private SymbolSign _symbol;
    private SymbolSize _size = SymbolSize.Default;

    private Vector3 _defaultPotentialTurnScale;
    
    private Image _image;
    
    private Action<DragableData> _onClickCell;

    private IAudioService _audioService;
    private IGameMetaProvider _gameMetaProvider;

    public void Construct(IAudioService audioService, IGameMetaProvider gameMetaProvider, Action<DragableData> onClickCell)
    {
      _onClickCell = onClickCell;
      _audioService = audioService;
      _gameMetaProvider = gameMetaProvider;

      _image = GetComponent<Image>();

      _defaultPotentialTurnScale = _potentialTurn.localScale;

      _image.DOFade(0f, 0);
      _image.transform.DOScale(0f, 0);
    }

    public SymbolSign GetSymbol() =>
      _symbol;

    public SymbolSize GetSize() =>
      _size;

    public void SetSymbol(DragableData dragableData)
    {
      _audioService.Play(AudioId.StandFigure);

      if (_image != null)
        _image.transform.DOPunchScale(-Vector3.one * .3f, .5f, 5);

      _symbol = dragableData.GetSymbol();
      _size = dragableData.GetSize();
      
      LoadCellModel(dragableData);
      
      _onClickCell.Invoke(dragableData);
    }

    public void HideNonPotentialTurn() => 
      _potentialTurn.DOScale(Vector3.zero, DurationPotentialTurn);

    public void ShowBackPotentialTurn() =>
      _potentialTurn.DOScale(_defaultPotentialTurnScale, DurationPotentialTurn);

    public void ShowCell(float delay)
    {
      _image.DOFade(1f, Duration).SetDelay(delay * .1f);
      _image.transform.DOScale(1f, Duration).SetDelay(delay * .1f)
        .OnPlay(() => { _audioService.Play(AudioId.Tile); });
    }

    private void LoadCellModel(DragableData dragableData)
    {
      _meshFilter.transform.localPosition = _gameMetaProvider.GetPosition(dragableData);
      _meshFilter.transform.localRotation = Quaternion.Euler(_gameMetaProvider.GetAngle(dragableData));
      
      _meshFilter.mesh = _gameMetaProvider.GetMesh(dragableData);
      _meshRenderer.material = _gameMetaProvider.GetMaterial(dragableData);


      if (_gameMetaProvider.GetAngle(dragableData) != Vector3.zero)
        _meshFilter.transform.DORotate(Vector3.right * -20, .1f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear);

      _meshRenderer.enabled = true;
    }
  }
}