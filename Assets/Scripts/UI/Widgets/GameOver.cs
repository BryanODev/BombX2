using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI.Tweens;

public class GameOver : Widget
{
    // Start is called before the first frame update

    ITweenEffect menuTranslationIn;
    ITweenEffect menuTranslationOut;
    [SerializeField] AnimationCurve menuTranslationAnimCurve;
    [SerializeField] float menuTranslationDuration = 0.5f;

    private void Awake()
    {
        menuTranslationIn = new TweenTranslate(GetComponent<RectTransform>(), new Vector2(0, -975), Vector2.zero, menuTranslationDuration, menuTranslationAnimCurve);
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        StartCoroutine(menuTranslationIn.Execute());
    }

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
