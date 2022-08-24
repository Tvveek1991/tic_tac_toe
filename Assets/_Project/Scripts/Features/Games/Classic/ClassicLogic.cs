using System.Collections.Generic;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Features.Games.Classic
{
  public class ClassicLogic : BaseFieldLogic
  {
    private const float Distance = 1.5f;
    private const string CubePath = "Features/GameScreen/Classic/Cube";

    private readonly List<Vector3> _matrix = new List<Vector3>();

    public ClassicLogic(IAssetProvider assetProvider, IAudioService audioService, Transform fieldParent) : base(assetProvider, audioService, fieldParent)
    {
      FieldParent = fieldParent;

      AudioService = audioService;
      AssetProvider = assetProvider;
    }

    public override void Construct()
    {
      CalculateField();

      var obj = AssetProvider.GetResource<GameObject>(CubePath);


      foreach (Vector3 t in _matrix)
      {
        GameObject newObj = Object.Instantiate(obj, t, Quaternion.identity, FieldParent);
        newObj.name = "Cube " + _matrix.IndexOf(t);
      }
    }

    public override void CalculateField(int height = 3, int width = 3)
    {
      float zPos = (float) height / 2;

      for (var j = 0; j < height; j++)
      {
        float xPos = -(float) width / 2;
        for (var k = 0; k < width; k++)
        {
          _matrix.Add(new Vector3(xPos, 0, zPos));
          xPos += Distance;
        }

        zPos -= Distance;
      }
    }
  }
}