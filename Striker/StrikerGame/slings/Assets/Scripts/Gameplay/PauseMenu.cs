using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject hud;
    public MenuButton resumeButton;
    public MenuButton exitButton;
    public MenuButton disconnectButton;
    public List<CNJoystick> joysticks;
    public GUISkin guiSkin;

    void Start()
    {
        resumeButton.OnAction += resumeButton_OnAction;
        exitButton.OnAction += exitButton_OnAction;
        disconnectButton.OnAction += disconnectButton_OnAction;
    }

    void OnEnable()
    {
        foreach (var item in joysticks)
        {
            item.gameObject.SetActive(false);
        }
        hud.gameObject.SetActive(false);
    }

    void disconnectButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            LiNGSClientManager lings = GameObject.FindObjectOfType<LiNGSClientManager>();
            if (lings != null)
            {
                lings.Client.Disconnect();
            }

        }
    }

    void exitButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            LiNGSClientManager lings = GameObject.FindObjectOfType<LiNGSClientManager>();
            if (lings != null)
            {
                lings.Client.Disconnect();
            }
            NetworkServer ns = GameObject.FindObjectOfType<NetworkServer>();
            if (ns != null)
            {
                ns.Shutdown();
            }
            Application.LoadLevel(0);
        }
    }

    void resumeButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            foreach (var item in joysticks)
            {
                item.gameObject.SetActive(true);
            }
            hud.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), 100.Scaled(), 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, "Game \"paused\"\n <i>You can't pause an online game!</i>");
    }
}
