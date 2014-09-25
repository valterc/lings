using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEndMenu : MonoBehaviour
{

    public string winner;
    public string winnerKills;
    public GameObject hud;
    public MenuButton exitButton;
    public List<CNJoystick> joysticks;
    public GUISkin guiSkin;

    void Start()
    {
        exitButton.OnAction += exitButton_OnAction;
    }

    void exitButton_OnAction(MenuButton button, MenuButton.ButtonState newState)
    {
        if (newState == MenuButton.ButtonState.Normal)
        {
            NetworkServer ns = GameObject.FindObjectOfType<NetworkServer>();
            if (ns != null)
            {
                ns.Shutdown();
            }
            Application.LoadLevel(0);
        }
    }    

    void Update()
    {

    }

    void OnGUI()
    {
        GUI.skin = guiSkin;
        guiSkin.label.fontSize = 22.Scaled();
        Rect labelRect = new Rect(Screen.width / 2 - 250.Scaled(), 120.Scaled(), 500.Scaled(), 100.Scaled());
        GUI.Label(labelRect, "Game is Over\n The Winner is: <i>" + winner + "</i> with " + winnerKills + " kills!");
    }

    public void GameEnded()
    {
        gameObject.SetActive(true);
        foreach (var item in joysticks)
        {
            item.gameObject.SetActive(false);
        }
    }
}
