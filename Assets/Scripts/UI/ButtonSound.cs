using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ButtonSound : MonoBehaviour
{
    [Inject]
    IAudioManager audioManager;

    public AudioClip buttonClickSound;

    public void OnButtonClick() 
    {
        if (buttonClickSound == null) { return; }

        audioManager?.PlayOneShotSound(buttonClickSound);
    }
}
