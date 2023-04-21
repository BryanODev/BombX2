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

    public override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        if (bombDefused) { return; }

        
        ResetBomb();
        bombDefused = true;

        //We add a score!
        gameModeScore.AddScore(1);

        spriteRenderer.color = defusedColor;
    }

    public void Explode() 
    {
        Debug.Log("Boom!");

        if (!isAlive) { return; }

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
