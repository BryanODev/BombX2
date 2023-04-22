using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coinsCollected;
    public bool firstTimePlaying; //If its the first time playing, enable the tutorial screen

    public PlayerData(int _coinsCollected, bool _firstTimePlaying)
    {
        coinsCollected = _coinsCollected;
        firstTimePlaying = _firstTimePlaying;
    }
}