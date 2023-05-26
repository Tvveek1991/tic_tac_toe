using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Localizator.Editor.Scripts
{
  [CreateAssetMenu(fileName = "Locale Fetcher", menuName = "Locale/Create Table Fetcher")]
  public class TableFetcher : ScriptableObject
  {
    private const string ExePath = "Assets/Localizator/Editor/locale_parser/main.exe";
    private const string CredentialsPath = "Assets/Localizator/Editor/locale_parser/tic-tac-toe-blocks-81519183-8ab2f6594032.json";

    [Tooltip("Id from Google Sheet url")]
    [SerializeField] private string tableId;
    [Tooltip("Name of a list in Google Sheet table")]
    [SerializeField] private string listName;
    
    [SerializeField] private Object outputDirectory;

    [CanBeNull] private string _json;
    
    [Button]
    public async void Fetch()
    {
      if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(outputDirectory)))
        throw new Exception("Fill outputDirectory field with valid directory");
      
      LaunchProcess();
      await LoadFetchedJson();
    }

    private void LaunchProcess()
    {
      var assetPath = Path.GetFullPath(ExePath);
      string arguments = $"\"{tableId}\" \"{listName}\" \"{Path.GetFullPath(CredentialsPath)}\" \"{Path.GetFullPath(AssetDatabase.GetAssetPath(outputDirectory))}\"";
      string infoWorkingDirectory = Path.GetDirectoryName(assetPath);

      Process process = new Process();
      process.StartInfo.FileName = assetPath;
      process.StartInfo.Arguments = arguments;
      process.StartInfo.WorkingDirectory = infoWorkingDirectory;
      process.Start();
    }

    private async Task LoadFetchedJson()
    {    
      await Task.Delay(1000);

      AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(outputDirectory)+"/locales.json");

      await Task.Delay(2000);
      
      SetMessage();
    }

    private void PrintError()
    {
      var errorType = JsonUtility.FromJson<ErrorType>(_json);
      
      if (_json != null && _json.Contains("Error"))
        Debug.Log($"[Table Fetcher] Error: {errorType.Error}");
      else 
        Debug.Log("[Table Fetcher] Done!");
    }

    private void SetMessage()
    {
      TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GetAssetPath(outputDirectory)+"/locales.json");
        if(textAsset)
          _json = textAsset.text;

      if (_json == null)
        Debug.Log("[Table Fetcher] Json not loaded yet");
      else
        PrintError();
    }
  }
}
