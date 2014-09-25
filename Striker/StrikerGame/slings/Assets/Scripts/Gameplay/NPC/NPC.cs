using UnityEngine;
using System.Collections;
using LiNGS.Common.GameLogic;

public class NPC : MonoBehaviour, INetworkedObject
{

    [NetworkedField]
    public float positionX;

    [NetworkedField]
    public float positionY;

    [NetworkedField]
    public float rotation;

    [NetworkedField]
    public string charName;

    [NetworkedField]
    public int color;

    [NetworkedField]
    public bool dead;

    [NetworkedField]
    public int health;

    [NetworkedField]
    public int kills;

    [NetworkedField]
    public bool disconnected;

    void Start()
    {

    }

    void Update()
    {
        Move();
        renderer.material.color = new Color32((byte)color, (byte)(color >> 8), (byte)(color >> 16), (byte)255);

        if (dead || disconnected)
        {
            renderer.enabled = false;
            //light.enabled = false;
        }
        else
        {
            renderer.enabled = true;
            //light.enabled = true;
        }

    }

    private void Move()
    {
        Vector3 newPosition = new Vector3(positionX, positionY, transform.position.z);
        Vector3 movement = transform.position - newPosition;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        if (movement.sqrMagnitude > .01f)
        {
            GetComponent<Animator>().StopPlayback();
        }
        else
        {
            GetComponent<Animator>().Play(0);
        }
    }

    public void LiNGSSetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
