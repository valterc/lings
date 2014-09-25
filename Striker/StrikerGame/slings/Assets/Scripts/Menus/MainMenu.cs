using UnityEngine;
using System.Collections;

public class MainMenu : Menu
{
    public MenuManager menuManager;
    public PlayMenu playMenu;
    public MenuButton playButton;
    public MenuButton optionsButton;
    public MenuButton exitButton;

    public override void Start()
    {
        playButton.OnAction += playButton_OnAction;
        optionsButton.OnAction += optionsButton_OnAction;
        exitButton.OnAction += exitButton_OnAction;
    }

    void exitButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            Application.Quit();
        }
    }

    void optionsButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        
    }

    void playButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(playMenu);
        }
    }

    public override void Update()
    {
    }
}
