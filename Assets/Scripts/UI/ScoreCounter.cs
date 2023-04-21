using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [Inject]
    IGameModeScore gameModeScore;

    TMP_Text scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (gameModeScore != null)
        {
            gameModeScore.OnGameScoreChangeDelegate += OnScoreChange;
        }
    }

    public void OnScoreChange(int score)
    {
        scoreText.text = score.ToString();
    }
}
