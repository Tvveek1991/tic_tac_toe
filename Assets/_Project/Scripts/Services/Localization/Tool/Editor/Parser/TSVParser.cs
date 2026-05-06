using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Services.Localization.Tool.Editor.Parser
{
  [CreateAssetMenu(fileName = "TSV Parser", menuName = "TSV Parser", order = 55)]
  public class TSVParser : ScriptableObject
  {
    [SerializeField] TSVParserDebug debug;

    [Header("[Input]")] [SerializeField] Object TSVText;

    [Header("[Output]")] [SerializeField] Object folder;

    List<string[]> splitedLines;

    public void CreateJSON()
    {
      if (!TSVText || !folder)
      {
        throw new System.Exception("Input/Output data is empty");
      }

      ReadText();
      CheckSplitedLines();

      string path = string.Format("{0}/{1}", AssetDatabase.GetAssetPath(folder), "locales.json");

      string json = WriteJSON();

      #region debug

      if (debug.createdJSON)
      {
        ColorizedDebugLog(json, "[Click here to see JSON]\n", Color.green, true);
      }

      #endregion

      using (StreamWriter outputFile = new StreamWriter(path))
      {
        outputFile.Write(json);
      }

      AssetDatabase.Refresh();

      ColorizedDebugLog("JSON is created", string.Empty, Color.green);
    }

    private void ReadText()
    {
      string path = AssetDatabase.GetAssetPath(TSVText);

      StreamReader reader = new StreamReader(path);

      splitedLines = new List<string[]>();
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        string[] splitedLine = line.Split('\t');

        splitedLines.Add(splitedLine);
      }

      reader.Close();
    }

    private void CheckSplitedLines()
    {
      if (splitedLines.Count.Equals(0))
      {
        throw new System.Exception("I can't read the file");
      }

      int length = splitedLines[0].Length;
      foreach (var line in splitedLines)
      {
        if (!line.Length.Equals(length))
        {
          foreach (var l in line)
          {
            Debug.Log(l);
          }

          throw new System.Exception("I have a problem with the line size" + $": ({line[0]}) ({splitedLines[0][line.Length - 1]}) -> ({line.Length} != {length})");
        }

        length = line.Length;
      }
    }

    private string WriteJSON()
    {
      int languagesCount = splitedLines[0].Length;
      int wordsCount = splitedLines.Count;

      string[,] Values = new string[languagesCount, wordsCount];

      for (int w = 0; w < wordsCount; w++)
      {
        for (int l = 0; l < languagesCount; l++)
        {
          Values[l, w] = splitedLines[w][l];

          #region debug

          /* Values[a, b]
           * line:    [a] == 0 ? key : language
           * column:  [b] 
           */

          if (debug.keys && l.Equals(0))
          {
            ColorizedDebugLog(Values[l, w], "[key]:\t", Color.magenta, true);
          }
          else if (debug.values && !l.Equals(0))
          {
            ColorizedDebugLog(Values[l, w], "[value]:\t", Color.cyan, true);
          }

          #endregion
        }
      }

      Dictionary<string, Dictionary<string, string>> languagesPack = new Dictionary<string, Dictionary<string, string>>();
      for (int l = 1; l < languagesCount; l++)
      {
        Dictionary<string, string> language = new Dictionary<string, string>();
        for (int w = 1; w < wordsCount; w++)
        {
          language.Add(Values[0, w], Values[l, w]);
        }

        languagesPack.Add(Values[l, 0], language);
      }

      return JsonConvert.SerializeObject(languagesPack, Formatting.Indented);
    }

    private void ColorizedDebugLog(string log, string prefix, Color color, bool onlyPrefixColorized = false)
    {
      byte r = (byte) (color.r * 255f);
      byte g = (byte) (color.g * 255f);
      byte b = (byte) (color.b * 255f);

      if (!onlyPrefixColorized)
      {
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}{4}</color>", r, g, b, prefix, log));
      }
      else
      {
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color> {4}", r, g, b, prefix, log));
      }
    }
  }

  [CustomEditor(typeof(TSVParser))]
  public class TSVParserEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      TSVParser parser = (TSVParser) target;

      if (GUILayout.Button("Create JSON"))
      {
        parser.CreateJSON();
      }
    }
  }

  [System.Serializable]
  internal struct TSVParserDebug
  {
    public bool keys;
    public bool values;
    public bool createdJSON;
  }
}