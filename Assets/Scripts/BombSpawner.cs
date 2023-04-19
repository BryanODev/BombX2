using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private Bomb bombPrefab;

    private ObjectPool<Actor> bombPool;

    [Inject]
    DiContainer container;
    
    [Inject] IGameModeEvents gameModeEvents;

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
        bombPool?.Get();
    }

    public void StopSpawner() 
    {
        Debug.Log("Stop Spawning Bombs!");
    }

    private Bomb CreateBomb()
    {
        //Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        Bomb bomb = container.InstantiatePrefab(bombPrefab, transform.position, Quaternion.identity, null).GetComponent<Bomb>();
        bomb.SetPool(bombPool);
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
        Destroy(bomb.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 10; i++)
            {
                bombPool?.Get();
            }
        }
    }
}