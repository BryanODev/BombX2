using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BombTeam
{
    public int bombTeamIndex;
    public Color bombColor;
}

[CreateAssetMenu(fileName = "New ColorPalletSystem", menuName = "GameVisuals/ColorPalletSystem", order = 0)]
public class ColorPallet : ScriptableObject
{
    public List<BombTeam> bombTeams;

    public void Initialize() 
    {
        bombTeams.Clear();

        GenerateRandomTriadricColor();
    }

    void GenerateRandomTriadricColor()
    {
        if (bombTeams.Count != 2) 
        {
            int numLeft = 2 - bombTeams.Count;

            for (int i = 0; i < numLeft; i++) 
            {
                bombTeams.Add(new BombTeam());
            }
        }

        float hue = Random.Range(0f, 1f);
        Color randomColor = Color.HSVToRGB(hue, .75f, 1f);

        bombTeams[0].bombTeamIndex = 0;
        bombTeams[0].bombColor = randomColor;
        bombTeams[0].bombColor.a = 1;

        float triadicHue = (hue + 0.667f) % 1;

        bombTeams[1].bombTeamIndex = 1;
        bombTeams[1].bombColor = Color.HSVToRGB(triadicHue, .75f, 1f);
        bombTeams[1].bombColor.a = 1;
    }
}
