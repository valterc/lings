using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Player player;
    public GUITexture frontBarTexture;
    public GUITexture backBarTexture;
    public GUITexture borderBarTexture;

    void Start()
    {
        StartCoroutine(ObtainPlayer());
        frontBarTexture.pixelInset = new Rect(-50.Scaled(), -10.Scaled(), 100.Scaled(), 20.Scaled());
        backBarTexture.pixelInset = new Rect(-50.Scaled(), -10.Scaled(), 100.Scaled(), 20.Scaled());
        borderBarTexture.pixelInset = new Rect(-52.Scaled(), -12.Scaled(), 104.Scaled(), 24.Scaled());
        guiText.fontSize = 15.Scaled();
        guiText.pixelOffset = new Vector2(-60.Scaled(), 0);
    }

    void Update()
    {
        if (player != null)
        {
            float width = Mathf.Lerp(frontBarTexture.pixelInset.width, player.health * 100.Scaled() / 10, 0.1f);
            frontBarTexture.pixelInset = new Rect(frontBarTexture.pixelInset.xMin, frontBarTexture.pixelInset.yMin, width, frontBarTexture.pixelInset.height);
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
