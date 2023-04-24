using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Gameplay.GameplayUtilities;

public class Bomb : Actor
{
    [Header("Bomb Sprite")]
    [SerializeField] Transform bombExplosionSprite;
    [SerializeField] Transform bombSprite;

    [Header("Bomb Gameplay Settings")]
    public bool autoStartTimer = true;
    public int bombID;
    public bool bombDefused;

    [SerializeField] float bombTimer = 8;
    float currentBombTimer;
    Coroutine bombTimerCoroutine;

    [Inject] IGameModeState gameModeState;
    [Inject] IGameModeScore gameModeScore;

    [Header("Bomb Animation")]

    [SerializeField] Color defusedColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] float bombAmplitude = .25f;
    [SerializeField] float bombDenotateSpeed = 1.25f;
    [SerializeField] float bombDenotateSpeedMultiplier = 2;
    [SerializeField] float bombDenotateScaleStart = 1.0f;

    SpriteRenderer spriteRenderer;
    bool isAlive = true;

    [Inject]
    IAudioManager audioManager;
    public AudioClip bombExplosionSFX;

    Material bombMaterialInstance;

    public override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bombMaterialInstance = spriteRenderer.material;

        currentBombTimer = bombTimer;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (autoStartTimer)
        {
            StartBombTimer();
        }
    }

    public void StartBombTimer() 
    {
        if (bombTimerCoroutine == null)
        {
            bombDefused = false;
            bombTimerCoroutine = StartCoroutine(BombTimer());
        }
    }

    public IEnumerator BombTimer() 
    {
        float timeElapsedRaw = 0;
        float timeElapsed = 0;

        while(currentBombTimer > 0) 
        {
            currentBombTimer -= Time.deltaTime;

            //Make bomb grow and shrink when only 3 seconds are left.
            if (currentBombTimer < 3) 
            {
                float sineValue = bombAmplitude * Mathf.Sin(timeElapsedRaw * (timeElapsed * bombDenotateSpeedMultiplier)) + bombDenotateScaleStart;
                sineValue = Mathf.Abs(sineValue);

                bombSprite.localScale = new Vector3(sineValue, sineValue, sineValue);
                timeElapsedRaw += Time.deltaTime;
                timeElapsed += Time.deltaTime * bombDenotateSpeed;
            }

            yield return null;
        }

        Explode();
    }

    public void DefuseBomb() 
    {
        if (bombDefused) { return; }

        
        ResetBomb();
        bombDefused = true;

        //We add a score!
        gameModeScore.AddScore(1);

        spriteRenderer.color = defusedColor;

        StopCoroutine(bounceCouroutine);
        StartCoroutine(FallObject(1));
    }

    public IEnumerator FallObject(float fallSpeed) 
    {
        Debug.Log("Fall");
        float timeElapsed = 0;
        Vector3 ToScale = new Vector3(0.05f, 0.05f, 0.05f);
        Vector3 currentScale = transform.localScale;

        while (transform.localScale != ToScale) 
        {
            transform.localScale = Vector3.Lerp(currentScale, ToScale, timeElapsed);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        Debug.Log("Fnished Falling");
        ReleaseToPool();

        yield return null;
    }

    public void Explode() 
    {
        Debug.Log("Boom!");

        if (!isAlive) { return; }

        canBeSelected = false;

        rb.velocity = Vector3.zero;

        audioManager?.PlayOneShotSound(bombExplosionSFX);
        StartCoroutine(GameplayUtilities.DoCameraShake(0.2f, .2f, 0));

        if (bombTimerCoroutine != null) 
        {
            StopCoroutine(bombTimerCoroutine);
        }

        isAlive = false;

        if (gameModeState.GameStarted && !gameModeState.GameEnded)
        {
            gameModeState?.EndGame();
        }

        //We disable the bomb sprite, and enable the bomb explosion sprite
        bombSprite.gameObject.SetActive(false);
        bombExplosionSprite.gameObject.SetActive(true);

        //StartCoroutine(ReleaseToPoolAfterSeconds(1f));
    }

    void ResetBomb() 
    {
        //Reset bomb timer
        ResetTimer();

        //Stop couroutines
        StopBombTimerCoroutine();

        //Reset color to white/default
        SetSpriteRendererColor(Color.white);

        //Reset Scaling of object and sprite
        ResetTransformScale();

        SetIsAlive(true);

        ResetBombSprite();
    }

    public override void OnDisable()
    {
        ResetBomb();
    }

    void ResetTimer() { currentBombTimer = bombTimer; }

    public void StopBombTimerCoroutine()
    {
        if (bombTimerCoroutine != null)
        {
            StopCoroutine(bombTimerCoroutine);
            bombTimerCoroutine = null;
        }
    }

    public void SetSpriteRendererColor(Color newColor) { spriteRenderer.color = newColor; }

    public void SetBombID(int id) 
    { 
        bombID = id; 
    }

    public void SetBombColor(Color bombColor) 
    {
        bombMaterialInstance.SetColor("_BombColor", bombColor);
    }

    public override void ResetTransformScale()
    {
        base.ResetTransformScale();
        bombSprite.localScale = Vector3.one; ;
    }

    public void SetIsAlive(bool newIsAlive) 
    {
        isAlive = newIsAlive;
    }

    public void ResetBombSprite() 
    {
        bombSprite.gameObject.SetActive(true);
        bombExplosionSprite.gameObject.SetActive(false);
    }
}
