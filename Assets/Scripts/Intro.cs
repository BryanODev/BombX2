using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    ScreenTransition transition;
    private void Start()
    {
        transition = GetComponentInChildren<ScreenTransition>();

        transition.TransitionOut();

        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame() 
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);

        yield return null;
    }
}
