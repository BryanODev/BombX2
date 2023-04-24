using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : Widget
{
    // Start is called before the first frame update
    public void RestartGame() 
    {
        playerUI.ScreenTransitionController.TransitionOut();

        StartCoroutine(RestartGameC());
    }

    IEnumerator RestartGameC() 
    {
        yield return new WaitForSeconds(.75f);

        //For now
        SceneManager.LoadScene(1);

        yield return null;
    }
}
