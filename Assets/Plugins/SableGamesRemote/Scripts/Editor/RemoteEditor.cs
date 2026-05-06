using System.IO;
using UnityEditor;
using UnityEngine;
using RemoteSettings = SableGamesRemote.Scripts.Data.RemoteSettings;

namespace SableGamesRemote.Scripts.Editor
{
  public static class RemoteEditor
  {
    [MenuItem("Sable Games/Create Remote Settings")]
    public static void Create()
    {
      var assetPath = $"Assets{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}Sable Games Remote Settings.asset";

      if (!AssetDatabase.IsValidFolder($"Assets{Path.DirectorySeparatorChar}Resources"))
        AssetDatabase.CreateFolder("Assets", "Resources");

      var settings = AssetDatabase.LoadAssetAtPath<RemoteSettings>(assetPath);

      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<RemoteSettings>();
        settings.name = "Sable Games Remote Settings";
        AssetDatabase.CreateAsset(settings, assetPath);
      }
      
      Selection.activeObject = settings;
      EditorGUIUtility.PingObject(settings);
    }
  }
}