using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Gameplay.GameplayUtilities;

public class Bomb : Actor
{
    [Header("Bomb Sprite")]
    SpriteRenderer bombSpriteRenderer;
    [SerializeField] Transform bombSparkleTransform;
    [SerializeField] Transform bombSprite;
    [SerializeField] Transform bombExplosionSprite;

    public SpriteRenderer BombSpriteRenderer { get { return bombSpriteRenderer; } set { bombSpriteRenderer = value; } }

    Collider2D bombCollider;

    [Header("Bomb Gameplay Settings")]
    public bool autoStartTimer = true;
    public int bombID;
    public bool bombDefused;

    [SerializeField] public float bombTimer = 8;
    float currentBombTimer;
    Coroutine bombTimerCoroutine;
    Coroutine fallObjectCoroutine;

    [Inject] IGameModeState gameModeState;
    [Inject] IGameModeScore gameModeScore;

    [Header("Bomb Animation")]

    [SerializeField] Color defusedColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] float bombDenotateAmplitude = .25f;
    [SerializeField] float bombDenotateSpeed = 1.25f;
    [SerializeField] float bombDenotateSpeedMultiplier = 2;
    [SerializeField] float bombDenotateScaleStart = 1.0f;

    bool isAlive = true;

    [Inject]
    IAudioManager audioManager;
    public AudioClip bombExplosionSFX;

    public override void Awake()
    {
        base.Awake();

        bombSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bombCollider = GetComponent<Collider2D>();

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

    private IEnumerator BombTimer() 
    {
        float timeElapsedRaw = 0;
        float timeElapsed = 0;

        while(currentBombTimer > 0) 
        {
            currentBombTimer -= Time.deltaTime;

            //Make bomb grow and shrink when only 3 seconds are left.
            if (currentBombTimer < 3) 
            {
                float sineValue = bombDenotateAmplitude * Mathf.Sin(timeElapsedRaw * (timeElapsed * bombDenotateSpeedMultiplier)) + bombDenotateScaleStart;
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

        bombDefused = true;
        ResetBomb();
        
        //We add a score!
        gameModeScore.AddScore(1);

        SetSpriteRendererColor(defusedColor);

        //Make sure we stop bouce coroutine so it doesn't fight with the FallObject coroutine.
        StopCoroutine(bounceCoroutine);
        StopAllCoroutines();
        fallObjectCoroutine = StartCoroutine(FallObject(1));
    }

    //Note: This function can be on Actor. For now we can leave it here, since we only have bombs as actors. But eventually, if we add more types of actors, this would be good in the Actor Class.
    private IEnumerator FallObject(float fallSpeed) 
    {
        canBeSelected = false;
        float timeElapsed = 0;
        Vector3 ToScale = new Vector3(0.05f, 0.05f, 0.05f);
        Vector3 currentScale = transform.localScale;

        while (transform.localScale != ToScale) 
        {
            transform.localScale = Vector3.Lerp(currentScale, ToScale, timeElapsed);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        Debug.Log("DISABLE!");
        //For now, since Fall Object means the object went trought the canal, we release to pool, disabling the gameobject.
        ReleaseToPool();

        yield return null;
    }

    public void Explode() 
    {
        if (!isAlive) { return; }

        isAlive = false;

        canBeSelected = false;

        //We 0 velocity to prevent the bomb explosion to slide over the level.
        rb.velocity = Vector3.zero;

        StopAllCoroutines();
        audioManager?.PlayOneShotSound(bombExplosionSFX);
        StartCoroutine(GameplayUtilities.DoCameraShake(0.2f, .2f, 0));

        if (bombTimerCoroutine != null) 
        {
            StopCoroutine(bombTimerCoroutine);
        }

        if (gameModeState.GameStarted && !gameModeState.GameEnded)
        {
            gameModeState?.EndGame();
        }

        //We disable the bomb sprite, and enable the bomb explosion sprite
        bombSprite.gameObject.SetActive(false);
        bombExplosionSprite.gameObject.SetActive(true);

        //If we were gonna give player lives, we can ReLeaseToPoolAfterSeconds(), so it goes back to pool to be reused.
        //ReleaseToPoolAfterSeconds(1f);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        ResetBomb();
    }

    //Resets the bomb. This is important since we reuse the bomb. When its released to pool / disabled, it will reset.
    void ResetBomb()
    {
        //Reset bomb timer
        ResetTimer();

        //Stop couroutines
        StopBombTimerCoroutine();

        //Reset Scaling of object and sprite
        ResetTransformScale();

        SetIsAlive(true);

        ResetBombSprite();

        isOnGround = false;
    }

    void ResetTimer() 
    { 
        currentBombTimer = bombTimer; 
    }

    public void StopBombTimerCoroutine()
    {
        if (bombTimerCoroutine != null)
        {
            StopCoroutine(bombTimerCoroutine);
            bombTimerCoroutine = null;
        }
    }

    public void SetSpriteRendererColor(Color newColor) 
    { 
        bombSpriteRenderer.color = newColor; 
    }

    public void SetBombID(int id) 
    { 
        bombID = id; 
    }

    //Not to be confused with SpriteRendere color. The bomb color uses materials properties, SpriteRenderer is on top.
    public void SetBombColor(Color bombColor) 
    {
        BombSpriteRenderer.material.SetColor("_BombColor", bombColor);
    }

    public override void ResetTransformScale()
    {
        base.ResetTransformScale();
        bombSprite.localScale = Vector3.one;
    }

    public void SetIsAlive(bool newIsAlive) 
    {
        isAlive = newIsAlive;
    }

    public void ResetBombSprite() 
    {
        bombSprite.gameObject.SetActive(true);
        bombExplosionSprite.gameObject.SetActive(false);

        //Reset color to white/default
        SetSpriteRendererColor(Color.white);
    }

    public void SetBombFireSparkPosition(Vector2 newPos) 
    {
        Vector3 pos = new Vector3(newPos.x, newPos.y, bombSparkleTransform.position.z);
        bombSparkleTransform.localPosition = pos;
    }
}
