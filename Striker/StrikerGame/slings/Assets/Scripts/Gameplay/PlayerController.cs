using UnityEngine;
using System.Collections;
using LiNGS.Common.GameLogic;

public class PlayerController : MonoBehaviour, INetworkedObject
{
    private LiNGSClientManager lings;
    private Vector3 joystickMove;

    public GameController gameController;
    public Player player;
    public CNJoystick movementJoystick;
    public CNJoystick rotationJoystick;


    [NetworkedField]
    public float positionX;

    [NetworkedField]
    public float positionY;

    [NetworkedField]
    public float rotation;

    [NetworkedField]
    public bool spawned;

    [NetworkedField]
    public bool shooting;

    private Vector2 newSpawnLocation;

    void Start()
    {
        lings = GameObject.FindObjectOfType<LiNGSClientManager>();
        lings.OnReceiveGameMessage += lings_OnReceiveGameMessage;

        movementJoystick.JoystickMovedEvent += movementJoystick_JoystickMovedEvent;
        rotationJoystick.JoystickMovedEvent += rotationJoystick_JoystickMovedEvent;
        rotationJoystick.FingerTouchedEvent += rotationJoystick_FingerTouchedEvent;
        rotationJoystick.FingerLiftedEvent += rotationJoystick_FingerLiftedEvent;

        movementJoystick.gameObject.SetActive(false);
        movementJoystick.gameObject.SetActive(true);
    }

    void rotationJoystick_FingerLiftedEvent()
    {
        shooting = false;
    }

    void rotationJoystick_FingerTouchedEvent()
    {
        shooting = true;
    }

    void lings_OnReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        if (message.Data[0].Object == "PlayerSpawn")
        {
            float x = float.Parse(message.Data[0].Value.Split(':')[0]);
            float y = float.Parse(message.Data[0].Value.Split(':')[1]);
            newSpawnLocation = new Vector2(x, y);
        }
    }

    void rotationJoystick_JoystickMovedEvent(Vector3 relativeVector)
    {
        relativeVector *= -1f;
        if (relativeVector != Vector3.zero && player != null && !player.dead)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(relativeVector.x, relativeVector.y, 6), Vector3.forward * -1);
            rotation = rot.eulerAngles.z;
            player.rotation = rot.eulerAngles.z;
        }
    }

    void movementJoystick_JoystickMovedEvent(Vector3 relativeVector)
    {
        relativeVector *= -.1f;
        if (player != null)
        {
            joystickMove = relativeVector;
        }
    }

    void Update()
    {
        if (!gameController.connected)
        {
            return;
        }

        if (newSpawnLocation != Vector2.zero)
        {
            player.positionX = newSpawnLocation.x;
            player.positionY = newSpawnLocation.y;
            player.transform.position = new Vector3(player.positionX, player.positionY, player.transform.position.z);

            positionX = newSpawnLocation.x;
            positionY = newSpawnLocation.y;

            newSpawnLocation = Vector2.zero;
        }

        if (player != null && !player.dead)
        {

            Vector3 movement = joystickMove;
            joystickMove = Vector3.zero;

            if (joystickMove == Vector3.zero)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    movement.y += -.1f;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    movement.y += .1f;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    movement.x += .1f;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    movement.x += -.1f;

                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    player.rotation += 2f;
                    rotation = player.rotation;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    player.rotation -= 2f;
                    rotation = player.rotation;
                }

                movement *= -2f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                shooting = true;
                /*
                ObjectManager om = GameObject.FindObjectOfType<ObjectManager>();
                Bullet b = (Bullet)om.CreateObject("Striker.Elements.Bullet", "bullet");
                b.startPositionX = player.transform.position.x;
                b.startPositionY = player.transform.position.y;
                b.rotation = player.transform.eulerAngles.z;
                b.directionX = Mathf.Sin(Mathf.Deg2Rad * player.transform.eulerAngles.z);
                b.directionY = -Mathf.Cos(Mathf.Deg2Rad * player.transform.eulerAngles.z);
                */
            }
            else
            {
                //shooting = false;
            }

            player.Move(movement);
            positionX = player.transform.position.x;
            positionY = player.transform.position.y;
        }
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        spawned = true;
        positionX = player.transform.position.x;
        positionY = player.transform.position.y;

        if (lings != null)
        {
            lings.Client.LogicProcessor.RegisterNetworkedObject(this, "PlayerController");
        }
    }

}
