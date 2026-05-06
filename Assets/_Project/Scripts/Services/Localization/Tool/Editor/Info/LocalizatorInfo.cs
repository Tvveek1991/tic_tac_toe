using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Services.Localization.Tool.Editor.Info
{
  [CreateAssetMenu(fileName = "Localizator Info", menuName = "Localizator Info", order = 55)]
  public class LocalizatorInfo : ScriptableObject
  {
    [MenuItem("Localizator/Info")]
    static void Init()
    {
      Show();
    }

    public void HowToUse()
    {
      Show();
    }

    private static void Show()
    {
      LocalizatorInfoWindow window = new LocalizatorInfoWindow();

      window.Show();
    }
  }

  [CustomEditor(typeof(LocalizatorInfo))]
  public class TSVParserEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      LocalizatorInfo parser = (LocalizatorInfo) target;

      if (GUILayout.Button("How To Use"))
      {
        parser.HowToUse();
      }
    }
  }
}