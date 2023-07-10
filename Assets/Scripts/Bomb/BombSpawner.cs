using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;


public class BombSpawner : MonoBehaviour
{
    [Inject]
    DiContainer container;

    [SerializeField] private Bomb bombPrefab;
    private ObjectPool<Actor> bombPool;
    private List<Bomb> bombs = new List<Bomb>(); //Used to hold all bombs spawned to then be able to explode them all on game lost.

    [Inject] IGameModeScore gameModeScore;
    [Inject] IGameModeEvents gameModeEvents;
    [Inject] IGameModeState gameModeState;
    [Inject] IGameInstance gameInstance;

    [SerializeField] BombSkinsLibrary bombSkinsLibrary;
    [SerializeField] BombSpawnPatternLibrary bombSpawnPatternLibrary;
    [SerializeField] BombSpawnerLevelLibrary bombSpawnLevelLibrary;

    private BombSpawnPattern lastPatternUsed;

    Coroutine bombSpawningCoroutine;
    Coroutine pattnerSpawnWaitTimeCoroutine;
    bool waitingOnPatternClear;
    bool wasWaitingOnPattern;

    [SerializeField] ColorPalletSystem colorPallet;

    private void Awake()
    {
        bombPool = new ObjectPool<Actor>(CreateBomb, OnGetBomb, OnReleaseBomb, OnDestroyBomb);
    }

    private void Start()
    {
        //Would be nice to spawn some bombs initially, and then use rather than spawning on use? For now we comment it. 
        //for(int i = 0; i < 20; i++) 
        //{
        //    Bomb bomb = CreateBomb();
        //    bomb.ReleaseToPool();
        //}

        if (gameModeEvents != null)
        {
            gameModeEvents.OnGameStartDelegate += StartSpawner;
            gameModeEvents.onGameEndDelegate += StopSpawner;
        }
    }

    public void StartSpawner() 
    {
        Debug.Log("Start Spawning Bombs!");

        bombSpawningCoroutine = StartCoroutine(SelectSpawnMethod());
    }

    public IEnumerator SelectSpawnMethod() 
    {
        while (!gameModeState.GameEnded)
        {
            if (!waitingOnPatternClear)
            {
                //If we were waiting on pattern, we wait a couple of seconds before spawning bombs again. So its not instantly spawn, and it keeps a consistent flow.
                if (wasWaitingOnPattern)
                {
                    wasWaitingOnPattern = false;
                    yield return new WaitForSeconds(GetCurrentLevel().timeBetweenBombs);
                }

                float singleBombOrPattern = Random.value;

                if (singleBombOrPattern < GetCurrentLevel().singleBombProbability)
                {
                    SpawnBomb(RandomPositionInArea(2f, 2f), Quaternion.identity).SetBombTimer(GetCurrentLevel().singleBombExplodeTime); //Spawn bomb in a random position inside the area and set the timer.
                }
                else
                {
                    SpawnRandomPatternByDifficulty(GetCurrentLevel().patternLevel, GetCurrentLevel());
                }

                yield return new WaitForSeconds(GetCurrentLevel().timeBetweenBombs);
            }

            //Debug.Log("Waiting on Pattern");
            yield return null;

        }

        yield return null;
    }

    Bomb SpawnBomb(Vector3 position, Quaternion rotation) 
    {
        //Debug.Log("Spawned bomb on pos: " + position);
        Bomb bomb = bombPool?.Get() as Bomb;
        int randomTeam = Random.Range(0, 2);

        if (bomb)
        {
            bomb.transform.SetPositionAndRotation(position, rotation);

            bomb.SetBombID(colorPallet.bombTeams[randomTeam].bombTeamIndex);
            bomb.SetBombColor(colorPallet.bombTeams[randomTeam].bombColor);

            return bomb;
        }

        return null;
    }

    public void SpawnPattern(string patternTag) 
    {
        lastPatternUsed = bombSpawnPatternLibrary.GetPatternByTag(patternTag);

        if (lastPatternUsed)
        {
            foreach (Vector2 point in lastPatternUsed.spawnPoints)
            {
                SpawnBomb(point, Quaternion.identity).SetBombTimer(GetCurrentLevel().patternBombExplodeTime);
            }
        }
    }

    public void SpawnRandomPatternByDifficulty(int difficulty, SpawnerLevel currentLevel)
    {
        List<BombSpawnPattern> patterns = bombSpawnPatternLibrary.GetPatternsByDifficulty(difficulty);
        int getRandomIndexPattern = Random.Range(0, patterns.Count);

        lastPatternUsed = patterns[getRandomIndexPattern];

        List<Bomb> bombsInPattern = new List<Bomb>();

        if (lastPatternUsed)
        {
            for (int i = 0; i < lastPatternUsed.spawnPoints.Length; i++) 
            {
                Bomb bomb = SpawnBomb(lastPatternUsed.spawnPoints[i], Quaternion.identity);
                bomb.bombTimer = currentLevel.patternBombExplodeTime;
                bombsInPattern.Add(bomb);
            }
        }

        waitingOnPatternClear = true;
        wasWaitingOnPattern = true;

        pattnerSpawnWaitTimeCoroutine = StartCoroutine(WaitForPatternToBeCompleted(bombsInPattern));
    }

    IEnumerator WaitForPatternToBeCompleted(List<Bomb> bombsInPattern) 
    {
        int bombInPatternCount = bombsInPattern.Count;
        int bombPatternDifused = 0;

        while (bombPatternDifused != bombInPatternCount) 
        {
            int bombDefusedInPattern = 0;

            for (int i = 0; i < bombsInPattern.Count; i++) 
            {
                if (bombsInPattern[i].bombDefused) 
                {
                    bombDefusedInPattern += 1;
                }
            }

            bombPatternDifused = bombDefusedInPattern;

            yield return null;
        }

        Debug.Log("All bombs Defused");
        waitingOnPatternClear = false;

        yield return null;
    }

    Vector2 RandomPositionInArea(float xHalfExt, float yHalfExt) 
    {
        float x = Random.Range(-xHalfExt, xHalfExt) / 2;
        float y = Random.Range(-yHalfExt, yHalfExt) / 2;

        Vector2 pos = new Vector2(x, y);

        return pos;
    }

    public void StopSpawner() 
    {
        Debug.Log("Stop Spawning Bombs!");

        StopAllCoroutines();

        StartCoroutine(ExplodeCurrentActiveBombs(.2f));
    }

    IEnumerator ExplodeCurrentActiveBombs(float delayPerExplosion) 
    {
        for (int i = 0; i < bombs.Count; i++)
        {
            bombs[i].StopBombTimerCoroutine();
            bombs[i].canBeSelected = false;
            yield return null;
        }

        bool skipExplosionDelay = true;

        for (int i = 0; i < bombs.Count; i++)
        {
            if (!skipExplosionDelay)
            {
                yield return new WaitForSeconds(delayPerExplosion);
            }

            if (!bombs[i].bombDefused && bombs[i].gameObject.activeSelf)
            {
                bombs[i].Explode();
                skipExplosionDelay = false;
            }
            else 
            {
                skipExplosionDelay = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Finished exploding");
        gameModeState.ShowGameOverScreen();

        yield return null;
    }

    private Bomb CreateBomb()
    {
        //Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        Bomb bomb = container.InstantiatePrefab(bombPrefab, transform.position, Quaternion.identity, null).GetComponent<Bomb>();
        bomb.SetPool(bombPool);

        BombSkin currentSkin = bombSkinsLibrary.GetBombSkinByName(gameInstance.PlayerDataSaved.currentBombSkinName);

        bomb.SetBombFireSparkPosition(currentSkin.bombSparklePosition);
        bomb.BombSpriteRenderer.sprite = currentSkin.bombSprite;
        bomb.BombSpriteRenderer.material = currentSkin.bombMaterial;


        bombs.Add(bomb);

        return bomb;
    }

    private void OnGetBomb(Actor bomb)
    {
        //Spawn bomb with player's settings
        //Load skins and assign here

        bomb.transform.position = transform.position;
        bomb.gameObject.SetActive(true);
    }

    private void OnReleaseBomb(Actor bomb)
    {
        bomb.transform.position = Vector3.zero;
        bomb.gameObject.SetActive(false);
    }

    private void OnDestroyBomb(Actor bomb)
    {
        Debug.Log("Destroy Bomb");
        Destroy(bomb.gameObject);
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 1));
    }

    public SpawnerLevel GetCurrentLevel() 
    {
        return bombSpawnLevelLibrary.GetCurrentLevelByScore(gameModeScore.GameScore);
    }
}