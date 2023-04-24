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
    [SerializeField] BombSpawnerLevel[] bombSpawnLevel;

    private void Awake()
    {
        bombPool = new ObjectPool<Actor>(CreateBomb, OnGetBomb, OnReleaseBomb, OnDestroyBomb);
    }

    private void Start()
    {
        if (gameModeEvents != null)
        {
            gameModeEvents.OnGameStartDelegate += StartSpawner;
            gameModeEvents.onGameEndDelegate += StopSpawner;
        }
    }

    public void StartSpawner() 
    {
        Debug.Log("Start Spawning Bombs!");

        StartCoroutine(SelectSpawnMethod());
    }

    public IEnumerator SelectSpawnMethod() 
    {
        int patternCooldown = 0;
        float secondsToWaitForSpawn = 2;

        while (!gameModeState.GameEnded)
        {
            if (gameModeScore.GameScore >= 0 && gameModeScore.GameScore < 10)
            {
                //Level 1
                SpawnBombInArea();
            }

            if (gameModeScore.GameScore >= 10 && gameModeScore.GameScore < 20)
            {
                //Level 2
                secondsToWaitForSpawn = 1.5f;

                float singleBombProbability = 0.75f;
                float singleBombOrPattern = Random.value;

                if (patternCooldown == 0) 
                {
                    singleBombOrPattern = 1;
                }

                if (singleBombOrPattern < singleBombProbability)
                {
                    SpawnBombInArea();
                }
                else
                {
                    patternCooldown = patternCooldownTime;
                    SpawnRandomPatternByDifficulty(0);
                    secondsToWaitForSpawn += 1;
                }
            }

            if (gameModeScore.GameScore >= 20 && gameModeScore.GameScore < 30)
            {
                //Level 3
                secondsToWaitForSpawn = 1.5f;

                float singleBombProbability = 0.75f;
                float singleBombOrPattern = Random.value;


                if (patternCooldown == 0)
                {
                    singleBombOrPattern = 1;
                }


                if (singleBombOrPattern < singleBombProbability)
                {
                    SpawnBombInArea();
                }
                else
                {
                    patternCooldown = patternCooldownTime;
                    SpawnRandomPatternByDifficulty(2);
                    secondsToWaitForSpawn += 1;
                }
            }

            if (gameModeScore.GameScore >= 30 && gameModeScore.GameScore < 40)
            {
                //Level 4
                secondsToWaitForSpawn = 1f;

                float singleBombProbability = 0.75f;
                float singleBombOrPattern = Random.value;

                if (patternCooldown == 0)
                {
                    singleBombOrPattern = 1;
                }

                if (singleBombOrPattern < singleBombProbability)
                {
                    SpawnBombInArea();
                }
                else
                {
                    patternCooldown = patternCooldownTime;
                    SpawnRandomPatternByDifficulty(4);
                    secondsToWaitForSpawn += 1;
                }
            }

            if (gameModeScore.GameScore >= 50)
            {
                //Level 5
                secondsToWaitForSpawn = 1f;

                float singleBombProbability = 0.65f;
                float singleBombOrPattern = Random.value;

                if (patternCooldown == 0)
                {
                    singleBombOrPattern = 1;
                }

                if (singleBombOrPattern < singleBombProbability)
                {
                    SpawnBombInArea();
                }
                else
                {
                    patternCooldown = patternCooldownTime;
                    SpawnRandomPatternByDifficulty(5);
                    secondsToWaitForSpawn += 1;
                }
            }

            patternCooldown -= 1;
            yield return new WaitForSeconds(secondsToWaitForSpawn);
        }

        yield return null;
    }


    void SpawnBomb(Vector3 position, Quaternion rotation) 
    {
        Bomb bomb = bombPool?.Get() as Bomb;
        int randomTeam = Random.Range(0, bombTeams.Length);

        if (bomb)
        {
            bomb.transform.SetPositionAndRotation(position, rotation);

            bomb.SetBombID(bombTeams[randomTeam].bombTeamIndex);
            bomb.SetBombColor(bombTeams[randomTeam].bombColor);
        }
    }

    void SpawnBombInArea()
    {
        SpawnBomb(RandomPositionInArea(2f, 2f), Quaternion.identity);
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

    public void SpawnRandomPatternByDifficulty(int difficulty)
    {
        List<BombSpawnPattern> patterns = bombSpawnPatternLibrary.GetPatternsByDifficulty(difficulty);
        int getRandomIndexPattern = Random.Range(0, patterns.Count);

        lastPatternUsed = patterns[getRandomIndexPattern];

        if (lastPatternUsed)
        {
            foreach (Vector2 point in lastPatternUsed.spawnPoints)
            {
                Bomb bomb = bombPool?.Get() as Bomb;
                bomb.transform.position = point;
            }
        }
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
            yield return null;
        }

        if (delayPerExplosion <= 0)
        {
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].Explode();
                yield return null;
            }
        }
        else 
        {
            for (int i = 0; i < bombs.Count; i++)
            {
                yield return new WaitForSeconds(delayPerExplosion);

                if (!bombs[i].bombDefused && bombs[i].gameObject.activeSelf)
                {
                    bombs[i].Explode();
                }

                yield return null;
            }
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
        bomb.gameObject.SetActive(true);
        bomb.transform.position = transform.position;
    }

    private void OnReleaseBomb(Actor bomb)
    {
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