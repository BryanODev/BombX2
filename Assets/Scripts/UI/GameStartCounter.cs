using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class GameStartCounter : MonoBehaviour
{
    [Inject] IGameModeState gameModeState;
    public Sprite[] NumberSprites;
    Image spriteRender;

    Coroutine gameStartTimer;

    private void Awake()
    {
        spriteRender = GetComponentInChildren<Image>();
    }

    public void StartGameTimer() 
    {
        if (gameStartTimer == null)
        {
            gameStartTimer = StartCoroutine(StartGameTimerC(3));
        }
    }

    public IEnumerator StartGameTimerC(float timeDuration)
    {
        int time = Mathf.RoundToInt(timeDuration);

        int lastIndex = time;

        while (timeDuration > 0)
        {
            timeDuration -= Time.deltaTime;

            time = Mathf.RoundToInt(timeDuration);

            if (lastIndex != time)
            {
                Sprite spriteNum = NumberSprites[time];
                spriteRender.sprite = spriteNum;
                lastIndex = time;

                if (lastIndex == 0)
                {
                    yield return new WaitForSeconds(1);
                }
            }

            yield return null;
        }

        gameModeState.StartGame();
        gameObject.SetActive(false);

        yield return null;
    }



}
