using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class HighScore : MonoBehaviour
{
    [Inject] IGameInstance gameInstance;
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        text.text = gameInstance.PlayerDataSaved.highScore.ToString();
    }

}
