using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    TMP_Text scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();

    }

    public void OnScoreChange(int score)
    {
        scoreText.text = score.ToString();
    }
}
