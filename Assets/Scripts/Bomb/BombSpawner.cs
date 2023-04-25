using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

[System.Serializable]
public class BombTeam
{
    public int bombTeamIndex;
    public Color bombColor;
}

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private Bomb bombPrefab;

    private ObjectPool<Actor> bombPool;
    public List<Bomb> bombs = new List<Bomb>();

    [Inject]
    DiContainer container;

    [Inject] IGameModeScore gameModeScore;
    [Inject] IGameModeEvents gameModeEvents;
    [Inject] IGameModeState gameModeState;

    [SerializeField] BombSpawnPatternLibrary bombSpawnPatternLibrary;
    private BombSpawnPattern lastPatternUsed;

    float bombSpawningSecondsToWait = 2;
    int patternCooldownTime = 5;

    [SerializeField] BombTeam[] bombTeams;
    [SerializeField] BombSpawnerLevelLibrary bombSpawnLevelLibrary;

    Coroutine bombSpawningCoroutine;
    Coroutine pattnerSpawnWaitTimeCoroutine;
    bool patternIsOn;

    public int defaultBombToSpawnFirst = 20;

    private void Awake()
    {
        bombPool = new ObjectPool<Actor>(CreateBomb, OnGetBomb, OnReleaseBomb, OnDestroyBomb);
    }

    private void Start()
    {
        //for(int i = 0; i < defaultBombToSpawnFirst; i++) 
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
            if (!patternIsOn)
            {
                SpawnerLevel currentLevel = bombSpawnLevelLibrary.GetCurrentLevelByScore(gameModeScore.GameScore);

                float singleBombOrPattern = Random.value;

                Debug.Log(currentLevel.singleBombProbability);

                if (singleBombOrPattern < currentLevel.singleBombProbability)
                {
                    Bomb bomb = SpawnBombInArea();
                    bomb.bombTimer = currentLevel.singleBombExplodeTime;
                }
                else
                {
                    SpawnRandomPatternByDifficulty(currentLevel.patternLevel, currentLevel);
                }

                yield return new WaitForSeconds(currentLevel.timeBetweenBombs);
            }

            //Debug.Log("Waiting on Pattern");
            yield return null;

        }

        yield return null;
    }


    Bomb SpawnBomb(Vector3 position, Quaternion rotation) 
    {
        Debug.Log("Spawned bomb on pos: " + position);
        Bomb bomb = bombPool?.Get() as Bomb;
        int randomTeam = Random.Range(0, bombTeams.Length);

        if (bomb)
        {
            bomb.transform.SetPositionAndRotation(position, rotation);

            bomb.SetBombID(bombTeams[randomTeam].bombTeamIndex);
            bomb.SetBombColor(bombTeams[randomTeam].bombColor);

            return bomb;
        }

        return null;
    }

    Bomb SpawnBombInArea()
    {
       return SpawnBomb(RandomPositionInArea(2f, 2f), Quaternion.identity);
    }

    public void SpawnPattern(string patternTag) 
    {
        lastPatternUsed = bombSpawnPatternLibrary.GetPatternByTag(patternTag);

        if (lastPatternUsed)
        {
            foreach (Vector2 point in lastPatternUsed.spawnPoints)
            {
                SpawnBomb(point, Quaternion.identity);
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

        patternIsOn = true;

        pattnerSpawnWaitTimeCoroutine = StartCoroutine(WaitForPatternToBeCompleted(bombsInPattern));
    }

    IEnumerator WaitForPatternToBeCompleted(List<Bomb> bombsInPattern) 
    {
        int bombCount = bombsInPattern.Count;
        int bombDifused = 0;

        while (bombDifused != bombCount) 
        {
            int bombTemp = 0;
            for (int i = 0; i < bombsInPattern.Count; i++) 
            {
                if (bombsInPattern[i].bombDefused) 
                {
                    bombTemp += 1;
                }
            }

            bombDifused = bombTemp;

            yield return null;
        }

        Debug.Log("All bombs Defused");
        patternIsOn = false;

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

        bombs.Add(bomb);

        return bomb;
    }

    private void OnGetBomb(Actor bomb)
    {
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
}