using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerUI 
{
    public ScoreCounter scoreCounter { get;}
    public ScreenTransition ScreenTransitionController { get; }
    public T GetMenu<T>() where T : Widget;
    public void OpenMenu<W>() where W : Widget;
}

public class PlayerUI : MonoBehaviour, IPlayerUI
{
    private Widget[] menus;
    public Widget currentMenu;
    public readonly Stack<Widget> _menuHistory = new Stack<Widget>();

    ScreenTransition screenTransitionController;
    public ScreenTransition ScreenTransitionController { get { return screenTransitionController; } private set { } }

    ScoreCounter _scoreCounter;
    public ScoreCounter scoreCounter { get { return _scoreCounter; } }

    private void Awake()
    {
        screenTransitionController = GetComponentInChildren<ScreenTransition>();
        _scoreCounter = GetComponentInChildren<ScoreCounter>();
    }

    private void Start()
    {
        menus = GetComponentsInChildren<Widget>();

        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].Initialize(this);
        }

        OpenMenu<MainScreen>();
        CloseAllMenusExclusive(GetMenu<MainScreen>());
    }

    public T GetMenu<T>() where T : Widget
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i] is T uiMenu)
            {
                return uiMenu;
            }
        }

        return null;
    }

    public void OpenMenu<W>() where W : Widget
    {
        W uiMenu = GetMenu<W>();

        if (uiMenu != null) 
        {
            //If we had a current menu, we push it to the menu history
            if (currentMenu) 
            {
                _menuHistory.Push(currentMenu);

                currentMenu.CloseMenu();
            }

            currentMenu = uiMenu;
            uiMenu.OpenMenu();
        }
    }

    public void OpenWidget(Widget widgetToOpen) 
    {
        if (widgetToOpen != null) 
        {
            //If we had a current menu, we push it to the menu history
            if (currentMenu)
            {
                _menuHistory.Push(currentMenu);

                currentMenu.CloseMenu();
            }

            currentMenu = widgetToOpen;
            widgetToOpen.OpenMenu();
        }
    }

    public void GoBackMenu() 
    {
        if (currentMenu) 
        {
            currentMenu.CloseMenu();
        }

        currentMenu = _menuHistory.Pop();
        currentMenu.OpenMenu();
    }
    public void CloseAllMenus() 
    {
        foreach (Widget menu in menus) 
        {
            menu.CloseMenu();
        }
    }

    public void CloseAllMenusExclusive(Widget menuToNotClose)
    {
        foreach (Widget menu in menus)
        {
            if (menu != menuToNotClose)
            {
                menu.CloseMenu();
            }
        }
    }
}