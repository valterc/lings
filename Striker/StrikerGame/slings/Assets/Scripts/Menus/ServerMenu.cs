using UnityEngine;
using System.Collections;
using System.Linq;
using LiNGS.Client;

public class ServerMenu : Menu
{

    public MenuManager menuManager;
    public PlayMenu playMenu;
    public MenuButton backButton;
    public MenuButton startButton;
    public GUISkin guiSkin;
    public NetworkServer server;
    public LiNGSClientManager lings;

    private string levelToLoad;

    public override void Start()
    {
        base.Start();

        backButton.OnAction += backButton_OnAction;
        startButton.OnAction += startButton_OnAction;

        lings.OnReceiveGameMessage += lings_OnReceiveGameMessage;

    }

    public override void Update()
    {
        if (levelToLoad != null)
        {
            Hypervisor.Default.levelName = levelToLoad;
            Application.LoadLevel("SceneGame");
        }
    }

    private void lings_OnReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        switch (message.Data[0].Object)
        {
            case "StartLevel":
                levelToLoad = message.Data[0].Value;
                Debug.Log("Load level: " + levelToLoad);
                break;
        } 
    }

    void startButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            server.StartGame();
        }
    }

    void backButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(playMenu);
        }
    }

    public override void OnEnter()
    {
        server.StartServer();
        lings.Connect(ClientProperties.Default, new ServerInfo(ServerInfo.DefaultLocal) { Port = 6666 });
    }

    public override void OnExit()
    {
        server.Shutdown();
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), Screen.height / 2 - 80.Scaled(), 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, "Server Info: <i>" + Network.player.ipAddress + "</i> : <i>" + server.strikerServer.game.server.Properties.ListenPort + "</i>");

        labelRect = new Rect(Screen.width / 2 - 250.Scaled(), Screen.height / 2 - 0, 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, "Connected clients: <b>" + server.strikerServer.game.server.LogicProcessor.GetConnectedClients().Count() + "</b>");
    }

}
