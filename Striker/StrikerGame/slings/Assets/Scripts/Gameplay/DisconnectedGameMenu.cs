using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisconnectedGameMenu : MonoBehaviour
{
    private LiNGSClientManager lings;

    public GameObject hud;
    public MenuButton reconnectButton;
    public MenuButton exitButton;
    public List<CNJoystick> joysticks;
    public GUISkin guiSkin;
    public string connectionProblem;

    private bool reconnecting;
    private string dots;
    private System.DateTime dotsChanged;

    void Start()
    {
        dots = ".";
        lings = GameObject.FindObjectOfType<LiNGSClientManager>();
        reconnectButton.OnAction += reconnectButton_OnAction;
        exitButton.OnAction += exitButton_OnAction;  
    }

    void exitButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            Application.LoadLevel(0);
        }
    }

    void reconnectButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            dots = ".";
            button.gameObject.SetActive(false);
            reconnecting = true;
            lings.Client.Connect();
        }
    }

    void OnEnable()
    {
        foreach (var item in joysticks)
        {
            item.gameObject.SetActive(false);
        }
        hud.gameObject.SetActive(false);

        reconnecting = false;
        dots = ".";
        dotsChanged = System.DateTime.Now;
    }

    void Update()
    {
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
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), 100.Scaled(), 500.Scaled(), 100.Scaled());

        if (reconnecting)
        {
            GUI.Label(labelRect, "Connecting to server" + dots);
        }
        else
        {
            GUI.Label(labelRect, "Connection problem: \n<color=#ff0000aa>" + (connectionProblem ?? "Bad connection, try reconnecting") + "</color>");
        }
        
    }

    public void ConnectionRestored()
    {
        reconnectButton.gameObject.SetActive(true);
        foreach (var item in joysticks)
        {
            item.gameObject.SetActive(true);
        }
        hud.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void FailedConnection()
    {
        reconnecting = false;
        reconnectButton.gameObject.SetActive(true);
    }

}
