using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public delegate void OnGameStart();
public delegate void OnGameEnd();

public enum GameState
{
    Initializing,
    WaitingToStart,
    StartingGame,
    InProcess,
    EndGame
}

public interface IGameModeScore 
{
    public int GameScore { get; }
    public void AddScore(int scoreToAdd);
}

public interface IGameModeState
{
    void StartGame();
    void EndGame();

    void ShowGameOverScreen();

    bool GameStarted { get; }
    bool GameEnded { get; }
}

public interface IGameModeEvents 
{
    public OnGameStart OnGameStartDelegate { get; set; }
    public OnGameEnd onGameEndDelegate { get; set; }
}

//This class will be the base of the gameplay loop. Will be incharge of stuff like spawning the player, starting the game, ending and more.
public class GameMode : MonoBehaviour, IGameModeState, IGameModeEvents, IGameModeScore
{
    //public static GameMode Instance { get; private set; }
    public GameState currentState;

    bool gameStarted;
    bool gameEnded;

    int gameScore;

    public int GameScore { get { return gameScore; } }

    public bool GameStarted { get { return gameStarted; } }

    public bool GameEnded { get { return gameEnded; } }

    [Inject]
    DiContainer container;

    [Header("GameMode Defaults")]

    [SerializeField ] PlayerController playerControllerPrefab;
    PlayerController playerController;

    [Inject]
    IPlayerUI playerUI;

    [Inject] IAudioManager audioManager;
    [Inject] IGameInstance gameInstance;

    //Delegates
    public OnGameStart onGameStart;
    public OnGameEnd onGameEnd;

    public OnGameStart OnGameStartDelegate { get { return onGameStart; } set { onGameStart += value; } }
    public OnGameEnd onGameEndDelegate { get { return onGameEnd; } set { onGameEnd += value; } }

    [SerializeField] ColorPallet colorPallet;


    public virtual void Awake()
    {
        currentState = GameState.Initializing;
        colorPallet.Initialize();
    }

    public void Start()
    {
        audioManager.PlayerRandomMusic();
        playerUI.ScreenTransitionController.TransitionIn();
    }

    public virtual void Update()
    {
        switch (currentState)
        {
            case GameState.Initializing:
                HandleInitializingGame();
                break;
            case GameState.WaitingToStart:
                break;
            case GameState.StartingGame:
                HandlingStartingGame();
                break;
            case GameState.InProcess:
                break;
            case GameState.EndGame:
                break;
        }
    }

    public virtual void HandleInitializingGame()
    {
        Debug.Log("Initializing Game");

        //On Game Finish Initialization, Change to Starting Game
        currentState = GameState.WaitingToStart;

        //Transition to the Scene
    }

    public void HandlingStartingGame() 
    {
 
    }

    public virtual void StartGame()
    {
        Debug.Log("Game Started!");

        gameStarted = true;
        currentState = GameState.InProcess;

        if (onGameStart != null)
        {
            onGameStart();
        }
    }

    //Can be useful if you want a special offer such as respawning the player for a ad video before ending the game.
    public virtual void StartHandlingFinishingGame()
    {

    }

    public virtual void EndGame()
    {
        //If the game already ended, no need of ending it again
        if (gameEnded) { return; }

        Debug.Log("Game Ended!");

        gameEnded = true;
        currentState = GameState.EndGame;

        if (onGameEnd != null)
        {
            onGameEnd();
        }

        audioManager.FadeMusicVolumen(0.5f, 0);

        if (gameScore > gameInstance.PlayerDataSaved.highScore)
        {
            gameInstance.PlayerDataSaved.highScore = gameScore;
        }

        gameInstance.SaveGameData();
    }

    public bool HasGameStarted()
    {
        return gameStarted;
    }

    public void TryAgain()
    {
        //Revive Player and continue
        currentState = GameState.InProcess;

        gameEnded = false;
    }

    public void AddScore(int scoreToAdd) 
    {
        gameScore += scoreToAdd;

        //Refresh PlayerUI.
        playerUI?.scoreCounter.OnScoreChange(gameScore);
    }

    public void ShowGameOverScreen()
    {
        playerUI.OpenMenu<GameOver>();
    }
}
