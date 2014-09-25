using UnityEngine;
using System.Collections;
using LiNGS.Common.GameLogic;

public class Bullet : MonoBehaviour, INetworkedObject
{

    [NetworkedField]
    public float startPositionX;

    [NetworkedField]
    public float startPositionY;

    [NetworkedField]
    public float directionX;

    [NetworkedField]
    public float directionY;

    [NetworkedField]
    public float rotation;

    private float timeAlive;

    void Start()
    {
        light.enabled = false;
        renderer.enabled = false;
    }

    void Update()
    {
        light.enabled = true;
        renderer.enabled = true;
        transform.eulerAngles = new Vector3(0, 0, rotation);
        transform.position = new Vector3(startPositionX + directionX * timeAlive * 8, startPositionY + directionY * timeAlive * 8, transform.position.z);
        timeAlive += Time.deltaTime;
    }
}