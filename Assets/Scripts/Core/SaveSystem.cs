using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public const string SaveDirectory = "/SaveData/";
    public const string FileName = "BombBox.bomb";

    public static PlayerData StartNewGame()
    {
        PlayerData newData = new PlayerData(0, true);
        return newData;
    }

    public static bool SavePlayer(PlayerData data) 
    {
        string path = Application.persistentDataPath + SaveDirectory;

        if (!Directory.Exists(path)) 
        {
            Directory.CreateDirectory(path);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + FileName, json);

        GUIUtility.systemCopyBuffer = path + FileName;

        return true;
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + SaveDirectory + FileName;

        PlayerData data;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log(path + " Data loaded.");
            return data;
        }
        else 
        {
            Debug.Log("No save data found at " + path + ". Creating new game.");
            return StartNewGame();
        }
    }
}
