using UnityEngine;
using System.Collections;
using LiNGS.Client;
using System.Linq;
using LiNGS.Common;

public class ConnectingMenu : Menu
{

    public LiNGSClientManager lings;
    public MenuManager menuManager;
    public ClientMenu clientMenu;
    public MenuButton backButton;
    public ConnectedMenu connectedMenu;
    public string ipAddress;
    public GUISkin guiSkin;

    private bool connecting;
    private string connectionError;
    private string dots;
    private System.DateTime dotsChanged;
    private bool connected;

    public override void Start()
    {
        backButton.OnAction += backButton_OnAction;
    }

    public override void OnEnter()
    {
        lings.OnConnectionAccepted += lings_OnConnectionAccepted;
        lings.OnConnectionRefused += lings_OnConnectionRefused;
        lings.OnUnableToConnect += lings_OnUnableToConnect;

        connected = false;
        backButton.gameObject.SetActive(false);
        connecting = true;
        connectionError = null;
        dots = ".";

        try
        {
            lings.Connect(ClientProperties.Default, new ServerInfo(ServerInfo.DefaultLocal) { IP = ipAddress, Port = 6666 });
        }
        catch (System.Exception e)
        {
            backButton.gameObject.SetActive(true);
            connecting = false;
            connectionError = e.Message;
        }
    }

    void backButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            menuManager.ChangeToMenu(clientMenu);
        }
    }

    void lings_OnUnableToConnect(string reason)
    {
        backButton.gameObject.SetActive(true);
        connecting = false;
        connectionError = reason;
    }

    void lings_OnConnectionRefused(LiNGS.Common.Network.NetworkMessage message)
    {
        backButton.gameObject.SetActive(true);
        connecting = false;
        connectionError = message.Data.FirstOrDefault(m => m.Object == LiNGSMarkers.Reason).Value;
    }

    void lings_OnConnectionAccepted(LiNGS.Common.Network.NetworkMessage message, bool usingSavedState)
    {
        connected = true;
    }

    public override void OnExit()
    {
        lings.OnConnectionAccepted -= lings_OnConnectionAccepted;
        lings.OnConnectionRefused -= lings_OnConnectionRefused;
        lings.OnUnableToConnect -= lings_OnUnableToConnect;
    }

    public override void Update()
    {
        if (connected)
        {
            menuManager.ChangeToMenu(connectedMenu);
            return;
        }

        if (System.DateTime.Now - dotsChanged > System.TimeSpan.FromSeconds(.5))
        {
            dotsChanged = System.DateTime.Now;
            if (dots.Length < 3)
            {
                dots += ".";
            }
            else
            {
                dots = ".";
            }
        }
        
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), Screen.height / 2, 500.Scaled(), 100.Scaled());

        if (connecting)
        {
            GUI.Label(labelRect, "Connecting to server" + dots);
        }
        else
        {
            GUI.Label(labelRect, "Connection to server failed: \n<color=#ff0000aa>" + connectionError + "</color>");
        }
    }

}
