using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Tweens;
using UnityEngine.UI;

public class SkinSelection : Widget, ITweenOwnerListener
{
    ITweenEffect skinSelectionTranslateIn;
    ITweenEffect skinSelectionTranslateOut;
    [SerializeField] Image backgroundImage;

    [SerializeField] Vector2 buttonTranslationStart;
    [SerializeField] Vector2 buttonTranslationEnd;

    [SerializeField] AnimationCurve settingScreenBackgroundFadeAnimCurve;
    [SerializeField] AnimationCurve buttonTranslationAnimCurve;
    [SerializeField] float animationDuration = 0.5f;

    bool isClosing = false;

    void Awake()
    {
        skinSelectionTranslateIn = new TweenTranslate(GetComponent<RectTransform>(), buttonTranslationStart, Vector2.zero, animationDuration, buttonTranslationAnimCurve, this);
        skinSelectionTranslateOut = new TweenTranslate(GetComponent<RectTransform>(), Vector2.zero, buttonTranslationEnd, animationDuration, buttonTranslationAnimCurve, this);
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        isClosing = false;
        StopAllCoroutines();
        StartCoroutine(skinSelectionTranslateIn.Execute());
    }

    public void CloseSkinSelect()
    {
        if (isClosing) { return; }

        StopAllCoroutines();
        StartCoroutine(skinSelectionTranslateOut.Execute());
        isClosing = true;
    }
    public void OnTweenFinish(ITweenEffect tween)
    {
        if (skinSelectionTranslateOut == tween)
        {
            playerUI.OpenMenu<MainScreen>();
            isClosing = false;
        }
    }
}
