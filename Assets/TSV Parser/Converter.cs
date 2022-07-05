using System;
using System.Collections.Generic;
using SimpleJSONByJeckich;
using UnityEngine;

namespace TSV_Parser
{
  public static class Converter
  {
    public static Dictionary<string, Dictionary<string, string>> GetData(string jsonTextFilePath)
    {
      string jsonText = Resources.Load<TextAsset>(jsonTextFilePath).text;
      Dictionary<string, Dictionary<string, string>> jsonData = new Dictionary<string, Dictionary<string, string>>();
      
      try
      {
        JSONClass json = JSON.Parse(jsonText).AsObject;

        foreach (string lang in json.m_Dict.Keys)
        {
          jsonData.Add(lang, new Dictionary<string, string>());

          foreach (string key in json.m_Dict[lang].AsObject.m_Dict.Keys)
            jsonData[lang].Add(key, json.m_Dict[lang].AsObject.m_Dict[key].Value);
        }
      }
      catch (Exception e)
      {
        Debug.LogError(e.StackTrace);
        
        if (Application.isEditor)
          Debug.LogWarning("The json converter is initialized incorrectly");
      }

      return jsonData;
    }

    public static string GetTextValue(Dictionary<string, Dictionary<string, string>> datas, string column, string line)
    {
      string res = string.Empty;

      try
      {
        res = datas[column][line];
      }
      catch (Exception e)
      {
        Debug.Log(e.StackTrace);
      }

      return res;
    }
    
    public static void LogAllColumnKeys(Dictionary<string, Dictionary<string, string>> data)
    {
      foreach (string key in data.Keys) 
        Debug.Log(key);
    }
    public static void LogAllLineKeys(Dictionary<string, Dictionary<string, string>> data, string columnKey)
    {
      foreach (string key in data[columnKey].Keys) 
        Debug.Log(key);
    }
  }
}