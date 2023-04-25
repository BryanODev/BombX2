using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int highScore;
    public bool firstTimePlaying; //If its the first time playing, enable the tutorial screen

    public PlayerData(int _coinsCollected, bool _firstTimePlaying)
    {
        highScore = _coinsCollected;
        firstTimePlaying = _firstTimePlaying;
    }
}