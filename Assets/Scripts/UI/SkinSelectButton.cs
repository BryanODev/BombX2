using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class SkinSelectButton : MonoBehaviour
{
    [Inject]
    IGameInstance gameInstance;

    Image buttonImage;
    [SerializeField] Sprite unselectedSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Transform skinPrice;
    public string bombSkinName;

    private Button skinSelectButton;
    private bool isUnlocked;

    SkinSelection screenSelection;

    private void Awake()
    {
        screenSelection = GetComponentInParent<SkinSelection>();
        skinSelectButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    public void SetIsUnlocked(bool newIsUnlocked) 
    {
        isUnlocked = newIsUnlocked;
    }

}
