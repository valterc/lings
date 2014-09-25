using UnityEngine;
using System.Collections;

public class ConnectedMenu : Menu
{

    public LiNGSClientManager lings;
    public MenuManager menuManager;
    public DisconnectedMenu disconnectedMenu;
    public MenuButton disconnectButton;
    public ClientMenu clientMenu;
    public GUISkin guiSkin;

    private string serverMessage;
    private bool disconnected;
    private string levelToLoad;

    public override void Start()
    {
        base.Start();

        disconnectButton.OnAction += disconnectButton_OnAction;
    }

    public override void OnEnter()
    {
        lings.OnReceiveEventMessage += lings_OnReceiveEventMessage;
        lings.OnReceiveGameMessage += lings_OnReceiveGameMessage;
        lings.OnDisconnected += lings_OnDisconnected;

        disconnected = false;
        serverMessage = "[Connected, Waiting for server]";
    }

    void lings_OnDisconnected(string reason)
    {
        if (reason != null)
        {
            disconnectedMenu.reason = reason;
            disconnected = true;
        }
    }

    void lings_OnReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        switch (message.Data[0].Object)
        {
            case "StartLevel":
                levelToLoad = message.Data[0].Value;
                Debug.Log("Load level: " + levelToLoad);
                break;
        } 
        serverMessage = message.Data[0].Object;
    }

    void lings_OnReceiveEventMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        serverMessage = message.Data[0].Object; 
    }

    public override void OnExit()
    {

    }

    void disconnectButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(clientMenu);
            lings.Client.Disconnect();
        }
    }

    public override void Update()
    {
        if (disconnected)
        {
            menuManager.ChangeToMenu(disconnectedMenu);
        }

        if (levelToLoad != null)
        {
            Hypervisor.Default.levelName = levelToLoad;
            Application.LoadLevel("SceneGame");
        }
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), Screen.height / 2, 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, serverMessage);
    }

}
