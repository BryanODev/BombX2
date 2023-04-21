using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Bomb : Actor
{
    public int bombID;

    Transform bombSprite;

    [SerializeField] float bombTimer = 8;
    float currentBombTimer;
    Coroutine bombTimerCoroutine;

    [Inject] IGameModeState gameModeState;

    [Header("Bomb Animation")]

    [SerializeField] Color defusedColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] float bombAmplitude = .25f;
    [SerializeField] float bombDenotateSpeed = 1.25f;
    [SerializeField] float bombDenotateSpeedMultiplier = 2;
    [SerializeField] float bombDenotateScaleStart = 1.0f;

    SpriteRenderer spriteRenderer;

    bool isAlive = true;

    public override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bombSprite = transform.GetChild(0);

        currentBombTimer = bombTimer;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        StartBombTimer();
    }

    public void StartBombTimer() 
    {
        if (bombTimerCoroutine == null)
        {
            bombTimerCoroutine = StartCoroutine(BombTimer());
        }
    }

    public IEnumerator BombTimer() 
    {
        float timeElapsed = 0;

        while(currentBombTimer > 0) 
        {
            currentBombTimer -= Time.deltaTime;

            //Make bomb grow and shrink when only 3 seconds are left.
            if (currentBombTimer < 3) 
            {
                float sineValue = bombAmplitude * Mathf.Sin(Time.time * (timeElapsed * bombDenotateSpeedMultiplier)) + bombDenotateScaleStart;
                sineValue = Mathf.Abs(sineValue);

                bombSprite.localScale = new Vector3(sineValue, sineValue, sineValue);
                timeElapsed += Time.deltaTime * bombDenotateSpeed;
            }

            yield return null;
        }

        Explode();
    }

    public void DefuseBomb() 
    {
        ResetBomb();
        spriteRenderer.color = defusedColor;
    }

    public void Explode() 
    {
        Debug.Log("Boom!");

        if (!isAlive) { return; }

        if (bombTimerCoroutine != null) 
        {
            StopCoroutine(bombTimerCoroutine);
        }

        isAlive = false;

        if (gameModeState.GameStarted && !gameModeState.GameEnded)
        {
            gameModeState?.EndGame();
        }

        //End the game!
        ReleaseToPool();
    }

    void ResetBomb() 
    {
        //Reset bomb timer
        currentBombTimer = bombTimer;

        //Stop couroutines
        if (bombTimerCoroutine != null)
        {
            StopCoroutine(bombTimerCoroutine);
        }

        bombTimerCoroutine = null;

        //Reset color to white/default
        spriteRenderer.color = Color.white;

        //Reset Scaling of object and sprite
        SetStartingScale();
        bombSprite.localScale = Vector3.one;

        isAlive = true;
    }

    public override void OnDisable()
    {
        ResetBomb();
    }
}
