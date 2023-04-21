using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private Bomb bombPrefab;

    private ObjectPool<Actor> bombPool;
    public List<Bomb> bombs = new List<Bomb>();

    [Inject]
    DiContainer container;
    
    [Inject] IGameModeEvents gameModeEvents;

    [SerializeField] BombSpawnPatternLibrary bombSpawnPatternLibrary;
    private BombSpawnPattern lastPatternUsed;

    int bombSpawningTimeMultiplier = 1;

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

        StartCoroutine(SpawnBombInArea());
    }

    IEnumerator SpawnBombInArea()
    {
        Bomb bomb = bombPool?.Get() as Bomb;
        
        if (bomb) 
        {
            bomb.transform.SetPositionAndRotation(RandomPositionInArea(2f, 2f), Quaternion.identity);
        }

        yield return new WaitForSeconds(2);

        StartCoroutine(SpawnBombInArea());

        yield return null;
    }

    public void SpawnPattern(string patternTag) 
    {
        lastPatternUsed = bombSpawnPatternLibrary.GetPatternByTag(patternTag);

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; i++)
            {
                Bomb bomb = bombPool?.Get() as Bomb;

                if (bomb)
                {
                    bomb.transform.SetPositionAndRotation(RandomPositionInArea(2f, 2.5f), Quaternion.identity);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(3, 3, 1));
    }
}