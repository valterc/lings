using UnityEngine;
using System.Collections;

public class PlayMenu : Menu
{
    public MenuManager menuManager;
    public MainMenu mainMenu;
    public ClientMenu clientMenu;
    public ServerMenu serverMenu;
    public MenuButton clientButton;
    public MenuButton serverButton;
    public MenuButton backButton;

    public override void Start()
    {
        base.Start();

        clientButton.OnAction += clientButton_OnAction;
        serverButton.OnAction += serverButton_OnAction;
        backButton.OnAction += backButton_OnAction;
    }

    void backButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(mainMenu);
        }
    }

    void serverButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(serverMenu);
        }
    }

    void clientButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(clientMenu);
        }
    }

    public override void Update()
    {
        base.Update();
    }

}
