using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Menu currentMenu;

    void Start()
    {
        currentMenu.gameObject.SetActive(true);
        currentMenu.OnEnter();
        Debug.developerConsoleVisible = true;
    }

    void Update()
    {

    }

    public void ChangeToMenu(Menu m)
    {
        currentMenu.OnExit();
        currentMenu.gameObject.SetActive(false);
        currentMenu = m;
        currentMenu.gameObject.SetActive(true);
        currentMenu.OnEnter();
    }

}
