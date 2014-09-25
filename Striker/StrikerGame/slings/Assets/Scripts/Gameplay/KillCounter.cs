using UnityEngine;
using System.Collections;

public class KillCounter : MonoBehaviour
{
    public Player player;

    void Start()
    {
        StartCoroutine(ObtainPlayer());
    }

    void Update()
    {
        guiText.fontSize = 15.Scaled();
        if (player != null)
        {
            guiText.text = "Player: " + player.charName + "   Kills: " + player.kills;
        }
    }

    IEnumerator ObtainPlayer()
    {
        yield return new WaitForSeconds(2);
        do
        {
            player = GameObject.FindObjectOfType<Player>();
            yield return new WaitForSeconds(1);
        } while (player == null);
    }

}
