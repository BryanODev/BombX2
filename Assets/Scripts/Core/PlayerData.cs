using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int highScore;
    public string currentBombSkinName;
    public List<string> bombSkinsUnlocked;
    public bool firstTimePlaying; //If its the first time playing, enable the tutorial screen

    public PlayerData(int _coinsCollected, string _currentBombSkinName, bool _firstTimePlaying)
    {
        highScore = _coinsCollected;
        currentBombSkinName = _currentBombSkinName;
        firstTimePlaying = _firstTimePlaying;
    }
}