using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public delegate void OnMeatAmmountChange(int newAmmount);

public interface IGameInstance
{
    PlayerData PlayerDataSaved { get; }
    GameSettings GetGameSettings { get; }

    void SetScore(int newScore);
    void SaveGameData();
}

public class GameInstance : IGameInstance
{

    DiContainer container;

    [SerializeField] private GameSettings gameSettings;
    public GameSettings GetGameSettings { get { return gameSettings; } private set { } }

    //Used to save and load
    private PlayerData playerDataSaved;
    public PlayerData PlayerDataSaved { get { return playerDataSaved; } }


    public GameInstance()
    {
        LoadSaveData();

        gameSettings = new GameSettings(true);
    }

    public void SetScore(int newScore)
    {
        if (newScore > playerDataSaved.highScore)
        {
            playerDataSaved.highScore = newScore;
        }
    }

    //For now, we just start a new game since this isn't really used. In future,
    //we might wanna make it delete the saved file rather than starting a new game on top.
    public void EraseGame()
    {
        playerDataSaved = SaveSystem.StartNewGame();
    }

    public void LoadSaveData()
    {
        Debug.Log("Loading Game...");

        playerDataSaved = SaveSystem.LoadPlayer();

        if (playerDataSaved == null)
        {
            playerDataSaved = SaveSystem.StartNewGame();
        }
    }

    public void SaveGameData()
    {
        Debug.Log("Saving Game");
        SaveSystem.SavePlayer(playerDataSaved);
    }
}
