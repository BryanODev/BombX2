using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class GameOverScore : MonoBehaviour
{
    TMP_Text scoreText;

    [Inject] IGameModeScore gameModeScore;

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();

        if (scoreText != null)
        {
            scoreText.text = gameModeScore.GameScore.ToString();
        }
    }

    private void OnEnable()
    {
        if (scoreText != null)
        {
            scoreText.text = gameModeScore.GameScore.ToString();
        }
    }
}
