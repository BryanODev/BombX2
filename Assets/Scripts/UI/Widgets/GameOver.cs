using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : Widget
{
    // Start is called before the first frame update
    public void RestartGame() 
    {
        //For now
        SceneManager.LoadScene(0);
    }
}
