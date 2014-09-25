using UnityEngine;
using System.Collections;
using LiNGS.Common.GameLogic;

public class Player : MonoBehaviour, INetworkedObject
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

    public Camera mainCamera;

    public CNJoystick movementJoystick;
    public CNJoystick rotationJoystick;

    void Start()
    {
        mainCamera = Camera.main;
        //movementJoystick.JoystickMovedEvent += movementJoystick_JoystickMovedEvent;
        //rotationJoystick.JoystickMovedEvent += rotationJoystick_JoystickMovedEvent;

        StartCoroutine(SetPlayerToController());    
    }

    void rotationJoystick_JoystickMovedEvent(Vector3 relativeVector)
    {
        LookTo(relativeVector * -1f);
    }

    void movementJoystick_JoystickMovedEvent(Vector3 relativeVector)
    {
        Move(relativeVector * .2f);
    }

    void Update()
    {
        /*
        Vector2 movement = Vector2.zero;

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

        movement *= 2f;

        Move(movement * -1f);
       */
        /*
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Quaternion rot = Quaternion.LookRotation(transform.position - mousePosition, Vector3.forward * -1);

        transform.rotation = rot;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); 
        */

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

        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        renderer.material.color = new Color32((byte)color, (byte)(color >> 8), (byte)(color >> 16), (byte)255);

        if (dead)
        {
            renderer.enabled = false;
            light.enabled = false;
        }
        else
        {
            renderer.enabled = true;
            light.enabled = true;
        }

    }

    public void Move(Vector3 movement)
    {
        positionX = transform.position.x + movement.x;
        positionY = transform.position.y + movement.y;

        if (movement.sqrMagnitude > .01f)
        {
            GetComponent<Animator>().StopPlayback();
        }
        else
        {
            GetComponent<Animator>().Play(0);
        }
    }

    public void LookTo(Vector3 relativeVector)
    {
        if (relativeVector != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(relativeVector.x, relativeVector.y, 6), Vector3.forward * -1);
            transform.rotation = rot;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z); 
        }
    }

    private IEnumerator SetPlayerToController()
    {
        yield return new WaitForSeconds(1.5f);

        PlayerController pc = GameObject.FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.SetPlayer(this);
        }

        GameStartingMessage msg = GameObject.FindObjectOfType<GameStartingMessage>();
        if (msg != null)
        {
            Destroy(msg.gameObject);
        }

    }

}
