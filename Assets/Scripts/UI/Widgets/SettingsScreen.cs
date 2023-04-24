using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zenject;

public class SettingsScreen : Widget, MuteButtonListener
{
    [Inject] IAudioManager audioManager;

    public override void OpenMenu()
    {
        base.OpenMenu();
    }

    public void CloseOptionScreen() 
    {
        playerUI.OpenMenu<MainScreen>();
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
}
