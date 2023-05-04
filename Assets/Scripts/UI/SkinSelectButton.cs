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

    private void OnEnable()
    {
        skinSelectButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        foreach (string bombName in gameInstance.PlayerDataSaved.bombSkinsUnlocked) 
        {
            if (bombSkinName.Equals(bombName)) 
            {
                isUnlocked = true;
                skinPrice.gameObject.SetActive(false);
            }
        }

        if (gameInstance.PlayerDataSaved.currentBombSkinName.Equals(bombSkinName)) 
        {
            buttonImage.sprite = selectedSprite;
        }
        else 
        {
            buttonImage.sprite = unselectedSprite;
        }
    }

    public void OnSkinSelect() 
    {
        gameInstance.PlayerDataSaved.currentBombSkinName = bombSkinName;
    }

}
