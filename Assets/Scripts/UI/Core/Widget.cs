using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Widget : MonoBehaviour
{
    protected PlayerUI playerUI;
    public bool isPersistent;

    public virtual void Initialize(PlayerUI _playerUI) 
    {
        playerUI = _playerUI;
    }

    public virtual void OpenMenu()
    {
        //OnPreOpenMenu();

        gameObject.SetActive(true);
    }

    ///// <summary>
    ///// Called before OpenMenu funciontality;
    ///// </summary>
    //public virtual void OnPreOpenMenu() 
    //{
    //
    //}

    public virtual void CloseMenu() 
    {
        //OnPreCloseMenu();

        gameObject.SetActive(false);
    }


    /// <summary>
    /// Called before CloseMenu funciontality;
    /// </summary
    //public virtual void OnPreCloseMenu() 
    //{
    //
    //}
}
