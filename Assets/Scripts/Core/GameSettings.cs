using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

[System.Serializable]
public class GameSettings
{
    public bool run60FPS = true;

    public GameSettings(bool runAt60)
    {
        run60FPS = runAt60;

        if (run60FPS)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = 30;
        }
    }
}
