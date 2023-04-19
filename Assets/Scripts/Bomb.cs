using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Bomb : Actor
{
    public int bombID;

    Transform bombSprite;

    [SerializeField] float bombTimer = 8;
    float currentBombTimer = 8;
    Coroutine bombTimerCoroutine;

    [Inject] IGameModeState gameModeState;

    [Header("Bomb Animation")]

    [SerializeField] Color defusedColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] float bombAmplitude = .25f;
    [SerializeField] float bombDenotateSpeed = 1.25f;
    [SerializeField] float bombDenotateSpeedMultiplier = 2;
    [SerializeField] float bombDenotateScaleStart = 1.0f;

    SpriteRenderer spriteRenderer;


    public override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bombSprite = transform.GetChild(0);
    }

    public override void Start()
    {
        base.Start();
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
        StopCoroutine(bombTimerCoroutine);
        spriteRenderer.color = defusedColor;

        //Make sure the scale is the starting one when difused
        SetStartingScale();
        bombSprite.localScale = Vector3.one;
    }

    public void Explode() 
    {
        Debug.Log("Boom!");

        gameModeState?.EndGame();

        //End the game!
        ReleaseToPool();
    }

    public override void OnDisable()
    {
        currentBombTimer = bombTimer;
        StopCoroutine(BombTimer());
        StopAllCoroutines();
        bombTimerCoroutine = null;

        spriteRenderer.color = Color.white;

        SetStartingScale();
        bombSprite.localScale = Vector3.one;
    }
}
