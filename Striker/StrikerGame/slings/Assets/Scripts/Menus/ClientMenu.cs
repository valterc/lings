using UnityEngine;
using System.Collections;
using System.Net;
using LiNGS.Client;
using System.Globalization;

public class ClientMenu : Menu
{

    public MenuManager menuManager;
    public PlayMenu playMenu;
    public ConnectingMenu connectingMenu;
    public MenuButton backButton;
    public MenuButton connectButton;
    public GUISkin guiSkin;

    private string ipAddress;

    public override void Start()
    {
        base.Start();

        backButton.OnAction += backButton_OnAction;
        connectButton.OnAction += connectButton_OnAction;
        ipAddress = "127.0.0.1";
    }

    void connectButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            connectingMenu.ipAddress = ipAddress;
            menuManager.ChangeToMenu(connectingMenu);
        }
    }

    void backButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(playMenu);
        }
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        guiSkin.textField.fontSize = 22.Scaled();
       
        Rect textAreaRect = new Rect(260.Scaled(),  Camera.main.WorldToScreenPoint(Vector3.one).y - (35 / 2).Scaled(), 360.Scaled(), 35.Scaled());
        ipAddress = GUI.TextField(textAreaRect, ipAddress, 15);
    }

}
