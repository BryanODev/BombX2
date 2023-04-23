using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainScreen : Widget
{
    [Inject]
    IGameModeState gameMode;

    public void StartGame() 
    {
        gameMode?.StartGame();
        playerUI.OpenMenu<PlayerHUD>();
    }
}
