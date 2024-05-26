using System.IO;
using SableGamesAds.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace SableGamesAds.Scripts.Editor
{
  public static class AdsEditor
  {
    [MenuItem("Sable Games/Create Ads Settings")]
    public static void CreateAdsSettings()
    {
      var name = "Sable Games Ads Settings";
      var assetPath = $"Assets{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}{name}.asset";

      Create(name, assetPath);
    }

    [MenuItem("Sable Games/Create Yandex Settings")]
    public static void CreateYandexSettings()
    {
      var name = "Yandex Ads Settings";
      var assetPath = $"Assets{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}{name}.asset";

      Create(name, assetPath);
    }

    private static void Create(string name, string assetPath)
    {
      if (!AssetDatabase.IsValidFolder($"Assets{Path.DirectorySeparatorChar}Resources"))
        AssetDatabase.CreateFolder("Assets", "Resources");

      var settings = AssetDatabase.LoadAssetAtPath<SableGamesAdsSettings>(assetPath);

      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<SableGamesAdsSettings>();
        settings.name = name;
        AssetDatabase.CreateAsset(settings, assetPath);
      }

      Selection.activeObject = settings;
      EditorGUIUtility.PingObject(settings);
    }
  }
}