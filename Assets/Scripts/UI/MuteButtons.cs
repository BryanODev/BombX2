using UnityEngine;
using UnityEngine.UI;
using Zenject;
public interface MuteButtonListener 
{
    void OnMuteButtonClicked(MuteButtonType buttonType);
}

public enum MuteButtonType
{
    Music,
    SFX
}

public class MuteButtons : MonoBehaviour
{
    [Inject] IAudioManager audioManager;

    [SerializeField] MuteButtonType muteButtonType;
    public MuteButtonType ButtonType { get { return muteButtonType; } private set { } }

    bool isMuted;

    MuteButtonListener muteButtonListener;  //Note: Not Ideal, but ok for now?
    Button muteButton;
    Image buttonImage;

    //Unmuted
    [Header("UnMuted State")]
    [SerializeField] Sprite unMutedUnSelected;
    [SerializeField] Sprite unMutedPressed;

    //Muted
    [Header("Muted State")]
    [SerializeField] Sprite mutedUnSelected;
    [SerializeField] Sprite mutedPressed;

    private void Awake()
    {
        muteButtonListener = GetComponentInParent<MuteButtonListener>();
        muteButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    public void OnEnable()
    {
        if (audioManager != null) 
        {
            switch (muteButtonType) 
            {
                case MuteButtonType.Music:
                    isMuted = audioManager.MusicMuted;
                    break;

                case MuteButtonType.SFX:
                    isMuted = audioManager.SFXMuted;
                    break;
            }

            ChangeButtonSprite();

        }
    }

    public void OnButtonSelect()
    {
        isMuted = !isMuted;

        ChangeButtonSprite();

        muteButtonListener.OnMuteButtonClicked(muteButtonType);
    }

    public void ChangeButtonSprite() 
    {
        if (isMuted)
        {
            buttonImage.sprite = mutedUnSelected;

            SpriteState state;
            state.pressedSprite = mutedPressed;
            state.selectedSprite = mutedUnSelected;

            muteButton.spriteState = state;
        }
        else
        {
            buttonImage.sprite = unMutedUnSelected;

            SpriteState state;
            state.pressedSprite = unMutedPressed;
            state.selectedSprite = unMutedUnSelected;

            muteButton.spriteState = state;
        }
    }
}
