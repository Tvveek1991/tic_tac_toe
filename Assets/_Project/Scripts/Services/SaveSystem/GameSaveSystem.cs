using _Project.Scripts.Services._data;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.SaveSystem
{
  public class GameSaveSystem : IGameSaveSystem
  {
    private const string SaveKey = "GameSaveData.1";

    private GameSaveData _gameSaveData;

    public GameSaveSystem()
    {
      string json = PlayerPrefs.GetString(SaveKey);
      _gameSaveData = JsonUtility.FromJson<GameSaveData>(json);
    }

    public GameSaveData Get() =>
      _gameSaveData ??= new GameSaveData();

    public void Save() =>
      PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_gameSaveData));
  }
}