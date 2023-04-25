using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternDebugView : MonoBehaviour
{
    public BombSpawnPattern pattern;

    public Color indexColor = new Color(0.4f, 1, 0.5f, 1);
    public Color sphereColor = Color.black;
    public int fontSize = 30;

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = indexColor;
        style.fontSize = fontSize;
        //style.alignment = TextAnchor.MiddleCenter;

        if (pattern != null) 
        {
            for(int i = 0; i < pattern.spawnPoints.Length; i++)
            {
                Vector2 point = pattern.spawnPoints[i];
                Gizmos.color = sphereColor;
                Gizmos.DrawWireSphere(point, 0.4f);
                Handles.Label(point, i.ToString(), style);
            }
        }
    }
}
