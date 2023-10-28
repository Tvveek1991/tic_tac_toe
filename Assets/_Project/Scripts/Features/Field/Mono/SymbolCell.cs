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
    private SymbolSign _symbol;
    private SymbolSize _size = SymbolSize.Default;

    private PotentialTurn _potentialTurn;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Image _image;

    private Action<DragableData> _onClickCell;

    public MeshFilter MeshFilter => _meshFilter;
    public MeshRenderer MeshRenderer => _meshRenderer;

    private IAudioService _audioService;

    public void Construct(IAudioService audioService, Action<DragableData> onClickCell)
    {
      _onClickCell = onClickCell;
      _audioService = audioService;

      _image = GetComponent<Image>();

      _meshFilter = GetComponentInChildren<MeshFilter>();
      _meshRenderer = GetComponentInChildren<MeshRenderer>();

      _potentialTurn = GetComponentInChildren<PotentialTurn>();
      _potentialTurn.Construct();

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
      _onClickCell.Invoke(dragableData);
    }

    public void ShowPotentialTurn(Mesh getMesh, Material getMaterial, Vector3 angle) =>
      _potentialTurn.ShowPotentialTurn(getMesh, getMaterial, angle);

    public void HidePotentialTurn() =>
      _potentialTurn.Hide();

    public void ShowCell(float delay)
    {
      _image.DOFade(1f, Duration).SetDelay(delay * .1f);
      _image.transform.DOScale(1f, Duration).SetDelay(delay * .1f)
        .OnPlay(() => { _audioService.Play(AudioId.Tile); });
    }
  }
}