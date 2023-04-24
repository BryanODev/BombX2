using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BombSpawn Pattern", menuName = "Bomb/BombSpawner/Pattern", order = 2)]
public class BombSpawnPattern : ScriptableObject
{
    public Vector2 globalPosition = Vector2.zero;
    public Vector2[] spawnPoints;
    public int spawnPointIndex;



    public Vector2 GetNextSpawnPoint() 
    {
        Vector2 point = Vector2.zero;

        if (spawnPoints.Length > spawnPointIndex)
        {
            point = spawnPoints[spawnPointIndex];

            spawnPointIndex++;
        }
        else
        {
            Debug.LogWarning("Pattern Index out of bound! Returning Vector2.Zero.");
        }


        return point;
    }

    public Vector2[] GetSpawnPoints() 
    {
        return spawnPoints;
    }

    public Vector2 GetSpawnPointByIndex(int index) 
    {
        if (spawnPoints.Length > index)
        {
            return spawnPoints[index];
        }
        else 
        {
            Debug.LogWarning("Pattern Index out of bound! Returning Vector2.Zero.");
        }
        
        return Vector2.zero;
    }

    public void Reset()
    {
        spawnPointIndex = 0;
    }
}
