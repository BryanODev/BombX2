using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BombSpawn Level library", menuName = "Bomb/BombSpawner/LevelLibrary", order = 0)]
public class BombSpawnerLevelLibrary : ScriptableObject
{
    public List<SpawnerLevel> levels = new List<SpawnerLevel>();

    public SpawnerLevel GetCurrentLevelByScore(int score) 
    {
        for (int i = 0; i < levels.Count; i++) 
        {
            if (score >= levels[i].minimunScore && score < levels[i].maximunScore) 
            {
                return levels[i];
            }
        }

        //return first level if null
        Debug.Log("Final Level");
        return levels[levels.Count];
    }

}

[System.Serializable]
public class SpawnerLevel 
{
    public float minimunScore;
    public float maximunScore;
    public float singleBombProbability;
    public float timeBetweenBombs;
    public int patternLevel;
    public float singleBombExplodeTime = 8;
    public float patternBombExplodeTime = 8;
}
