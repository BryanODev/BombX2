using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BombSpawn Level", menuName = "Bomb/BombSpawner/Level", order = 0)]
public class BombSpawnerLevel : ScriptableObject
{
    public float singleBombProbability;
    public float timeBetweenBombs;
}
