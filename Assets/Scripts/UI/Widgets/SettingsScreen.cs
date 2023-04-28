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

    [SerializeField] Image backgroundImage;
    ITweenEffect settingScreenBackgroundFadeIn;
    ITweenEffect settingScreenBackgroundFadeOut;
    [SerializeField] AnimationCurve settingScreenBackgroundFadeAnimCurve;

    CanvasGroup canvasGroup;
    float alpha;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        settingScreenBackgroundFadeIn = new TweenCanvasGroupAlpha(canvasGroup, 0, 1.0f, 0.5f, settingScreenBackgroundFadeAnimCurve, this);
        settingScreenBackgroundFadeOut = new TweenCanvasGroupAlpha(canvasGroup, 1.0f, 0, 0.5f, settingScreenBackgroundFadeAnimCurve, this);
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        StopAllCoroutines();
        StartCoroutine(settingScreenBackgroundFadeIn.Execute());
    }

    public void CloseOptionScreen() 
    {
        StopAllCoroutines();
        StartCoroutine(settingScreenBackgroundFadeOut.Execute());
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
        if(settingScreenBackgroundFadeOut == tween)
        {
            playerUI.OpenMenu<MainScreen>();
        }
    }
}
