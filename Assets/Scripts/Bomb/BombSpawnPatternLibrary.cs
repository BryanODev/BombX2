using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pattern Library", menuName = "Bomb/BombSpawner/PatternLibrary", order = 1)]
public class BombSpawnPatternLibrary : ScriptableObject
{
    public List<BombSpawnPatternData> patterns = new List<BombSpawnPatternData>();

    public BombSpawnPattern GetPatternByTag(string tag) 
    {
        foreach (BombSpawnPatternData bombSpawnPatternData in patterns) 
        {
            if (bombSpawnPatternData.patternTag.Equals(tag)) 
            {
                return bombSpawnPatternData.pattern;
            }
        }

        Debug.LogWarning("Pattern with tag: " + tag + " doesn't exist.");
        return null;
    }

    public BombSpawnPattern GetPatternByIndex(int index)
    {
        if (patterns.Count > index) 
        {
            return patterns[index].pattern;
        }

        Debug.LogWarning("Pattern with index: " + index + " doesn't exist.");
        return null;
    }

    public List<BombSpawnPattern> GetPatternsByDifficulty(int difficulty) 
    {
        List<BombSpawnPattern> patternsWithDifficulty = new List<BombSpawnPattern> ();

        foreach (BombSpawnPatternData bombSpawnPatternData in patterns)
        {
            if (bombSpawnPatternData.difficulty <= difficulty)
            {
                patternsWithDifficulty.Add(bombSpawnPatternData.pattern);
            }
        }

        return patternsWithDifficulty;
    }

}

[System.Serializable]
public class BombSpawnPatternData 
{
    public string patternTag;
    public BombSpawnPattern pattern;
    public int difficulty;
}