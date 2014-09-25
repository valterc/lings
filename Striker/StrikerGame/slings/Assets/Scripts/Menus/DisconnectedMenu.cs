using UnityEngine;
using System.Collections;

public class DisconnectedMenu : Menu
{

    public LiNGSClientManager lings;
    public MenuManager menuManager;
    public ClientMenu clientMenu;
    public ConnectingMenu connectingMenu;
    public MenuButton backButton;
    public MenuButton connectButton;
    public GUISkin guiSkin;
    public string reason;

    public override void Start()
    {
        base.Start();

        backButton.OnAction += backButton_OnAction;
        connectButton.OnAction += connectButton_OnAction;
    }

    void connectButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(connectingMenu);
        }
    }

    void backButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(clientMenu);
        }
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), Screen.height / 2, 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, "Disconnected from server: \n" + reason);
    }

}
