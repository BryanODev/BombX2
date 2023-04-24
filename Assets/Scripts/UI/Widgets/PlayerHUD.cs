using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : Widget
{
    GameStartCounter counter;

    private void Start()
    {
        counter = GetComponentInChildren<GameStartCounter>();
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        counter.StartGameTimer();
    }
}
