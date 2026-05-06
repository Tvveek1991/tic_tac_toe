using System.IO;
using SableGamesAnalytics.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace SableGamesAnalytics.Scripts.Editor
{
  public static class AnalyticsEditor
  {
    [MenuItem("Sable Games/Create Analytics Settings")]
    public static void Create() => 
      CreateFile("Sable Games Analytics Settings");

    [MenuItem("Sable Games/Create Analytics Settings IOS")]
    public static void CreateIOS() => 
      CreateFile("Sable Games Analytics Settings IOS");

    private static void CreateFile(string name)
    {
      var assetPath = $"Assets{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}{name}.asset";

      if (!AssetDatabase.IsValidFolder($"Assets{Path.DirectorySeparatorChar}Resources"))
        AssetDatabase.CreateFolder("Assets", "Resources");

      var settings = AssetDatabase.LoadAssetAtPath<AnalyticsSettings>(assetPath);

      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<AnalyticsSettings>();
        settings.name = name; 
        AssetDatabase.CreateAsset(settings, assetPath);
      }
      
      Selection.activeObject = settings;
      EditorGUIUtility.PingObject(settings);
    }
  }
}