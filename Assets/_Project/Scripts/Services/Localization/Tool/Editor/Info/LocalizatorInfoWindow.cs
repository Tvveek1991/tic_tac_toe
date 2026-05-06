using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Services.Localization.Tool.Editor.Info
{
  public class LocalizatorInfoWindow : EditorWindow
  {
    void OnGUI()
    {
      LocalizatorInfoWindow window = (LocalizatorInfoWindow) GetWindow(typeof(LocalizatorInfoWindow));

      window.maxSize = new Vector2(500f, 400f);
      window.minSize = window.maxSize;

      GUILayout.BeginVertical();

      GUIStyle guiStyle;

      guiStyle = EditorStyles.boldLabel;
      guiStyle.alignment = TextAnchor.UpperCenter;
      GUILayout.Label("Как использовать TSV Parser", guiStyle);

      GUILayout.Space(10);

      guiStyle = EditorStyles.wordWrappedLabel;
      guiStyle.alignment = TextAnchor.MiddleLeft;
      guiStyle.fontStyle = FontStyle.Italic;
      GUILayout.Label("> Открой google таблицу с локализациями", guiStyle);
      GUILayout.Label("> Нажми: Файл/Скачать/TSV(текущий лист)", guiStyle);
      GUILayout.Label("> Скаченный файл перетащи в TSV Parser->[Input] и нажми [Create JSON]", guiStyle);
      GUILayout.Space(5);
      GUILayout.Label("Путь: .../Localizator/Editor/Parser/TSV Parser", guiStyle);

      GUILayout.Space(10);

      guiStyle = EditorStyles.boldLabel;
      guiStyle.alignment = TextAnchor.UpperCenter;
      GUILayout.Label("Использование локализатора", guiStyle);

      GUILayout.Space(10);

      guiStyle = EditorStyles.wordWrappedLabel;
      guiStyle.alignment = TextAnchor.MiddleLeft;
      guiStyle.fontStyle = FontStyle.Italic;
      GUILayout.Label("> Чтобы сработала автоматическая локализация на сцене должен быть скрипт [LocalizationManager]", guiStyle);
      GUILayout.Label("> Далее как обычно: на объект с текстом вешай скрипт [LocalizeText] и вставляй ключ", guiStyle);

      GUILayout.EndVertical();
    }
  }
}