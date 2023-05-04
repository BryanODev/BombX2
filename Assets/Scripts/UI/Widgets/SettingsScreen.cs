using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zenject;
using UI.Tweens;

public class SettingsScreen : Widget, MuteButtonListener, ITweenOwnerListener
{
    [Inject] IAudioManager audioManager;

    ITweenEffect settingScreenBackgroundFadeIn;
    ITweenEffect settingScreenBackgroundFadeOut;
    ITweenEffect buttonTranslateIn;
    ITweenEffect buttonTranslateOut;
    [SerializeField] Image backgroundImage;
    [SerializeField] RectTransform buttonGrid;

    [SerializeField] Vector2 buttonTranslationStart;
    [SerializeField] Vector2 buttonTranslationEnd;

    [SerializeField] AnimationCurve settingScreenBackgroundFadeAnimCurve;
    [SerializeField] AnimationCurve buttonTranslationAnimCurve;
    [SerializeField] float animationDuration = 0.5f;

    bool isClosing = false;

    void Awake()
    {
        settingScreenBackgroundFadeIn = new TweenImageAlpha(backgroundImage, 0, 0.8f, animationDuration, settingScreenBackgroundFadeAnimCurve, this);
        settingScreenBackgroundFadeOut = new TweenImageAlpha(backgroundImage, 0.8f, 0, animationDuration, settingScreenBackgroundFadeAnimCurve, this);

        buttonTranslateIn = new TweenTranslate(buttonGrid, buttonTranslationStart, Vector2.zero, animationDuration, buttonTranslationAnimCurve, this);
        buttonTranslateOut = new TweenTranslate(buttonGrid, Vector2.zero, buttonTranslationEnd, animationDuration, buttonTranslationAnimCurve, this);
    }

    void OnEnable()
    {
        Color color = backgroundImage.color;
        color.a = 0;
        backgroundImage.color = color;
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        StopAllCoroutines();
        StartCoroutine(settingScreenBackgroundFadeIn.Execute());
        ButtonTranslateIn();
    }

    void ButtonTranslateIn()
    {
        StartCoroutine(buttonTranslateIn.Execute());
    }

    void ButtonTranslateOut()
    {
        StartCoroutine(buttonTranslateOut.Execute());
    }

    public void CloseOptionScreen() 
    {
        if (isClosing) { return; }

        StopAllCoroutines();
        StartCoroutine(settingScreenBackgroundFadeOut.Execute());
        ButtonTranslateOut();
        isClosing = true;
    }

    public void OnMuteButtonClicked(MuteButtonType button)
    {
        switch (button) 
        {
            case MuteButtonType.Music:

                if (audioManager != null)
                {
                    audioManager.ToggleMusicMute();
                }

                break;
            case MuteButtonType.SFX:

                if (audioManager != null)
                {
                    audioManager.ToggleSFXMute();
                }

                break;
        }
    }

    public void OnTweenFinish(ITweenEffect tween)
    {
        if (settingScreenBackgroundFadeOut == tween)
        {
            playerUI.OpenMenu<MainScreen>();
            isClosing = false;
        }
    }

}
