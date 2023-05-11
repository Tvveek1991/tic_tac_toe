using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Features.Customization.Interface;
using _Project.Scripts.MonoBehaviours.Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Features.Customization.Serialized
{
  [CreateAssetMenu(fileName = "Skins Config", menuName = "Skins/Config", order = 0)]
  public class SkinsConfig : ScriptableObject
  {
    [SerializeField] private string dataPath = string.Empty;
    [SerializeField] private string dataGoPath = string.Empty;

    [SerializeField] private List<ItemId> _itemIds;
    [SerializeField] private List<SkinDataConfig> _skinsData;

    public ISkinData[] SkinsData => _skinsData.ToArray();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AllValidate()
    {
#if UNITY_EDITOR
      Debug.Log("All Validate");
      
      SkinsConfig[] configs = Resources.LoadAll<SkinsConfig>("");
      
      foreach (var c in configs) 
        c.Validate();
#endif
    }

    [Button("Validate", ButtonSizes.Large)]
    public void Validate()
    {
      GenerateGuid();
      CopyIdToItems();
      
#if UNITY_EDITOR
      AssetDatabase.SaveAssets();
#endif
    }

    private void CopyIdToItems()
    {
      for (var i = 0; i < _skinsData.Count; i++)
      {
        _itemIds[i].Id = _skinsData[i].Guid;
#if UNITY_EDITOR
        EditorUtility.SetDirty(_itemIds[i]);
#endif
      }
    }

    [Button("Clean")]
    public void Clean()
    {
      _skinsData = new List<SkinDataConfig>();
      _itemIds = new List<ItemId>();
    }
    
    private void GenerateGuid()
    {
      _skinsData.ToList().ForEach(current =>
      {
        if (current.Guid == string.Empty)
          Generate(current, " empty");
        else
        {
          if (_skinsData.Any(other => other != current && other.Guid == current.Guid))
            Generate(current," justNew");
        }
      });

      void Generate(SkinDataConfig skinData, string message)
      {
        Debug.Log(skinData.Guid + message);
        skinData.Guid = Guid.NewGuid().ToString();
#if UNITY_EDITOR
        EditorUtility.SetDirty(skinData);
#endif
      }
    }
  }
}