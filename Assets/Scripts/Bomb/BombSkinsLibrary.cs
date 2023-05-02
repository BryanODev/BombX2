using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BombSkins Library", menuName = "Bomb/BombSkins/BombSkinsLibrary", order = 0)]
public class BombSkinsLibrary : ScriptableObject
{
    public List<BombSkin> bombSkinList;

    public BombSkin GetBombSkinByName(string bombSkinName) 
    {
        foreach (BombSkin bombSkin in bombSkinList) 
        {
            if (bombSkin.bombSkinName.Equals(bombSkinName)) 
            {
                return bombSkin;
            }
        }

        Debug.LogWarning("The skin named: " + bombSkinName + " was not found in the library! Returning default skin.");

        return bombSkinList[0];
    }
}

[System.Serializable]
public class BombSkin
{
    public string bombSkinName;
    public Sprite bombSprite;
    public Vector2 bombSparklePosition = new Vector2(2.2f, 1);
    public Material bombMaterial;
}