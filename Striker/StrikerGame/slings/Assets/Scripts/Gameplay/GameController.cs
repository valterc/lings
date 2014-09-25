using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    private enum ConnectionState
    {
        None, 
        Connecting, 
        Connected,
        Disconnected,
        Failed
    }

    private LiNGSClientManager lings;

    public LevelLoader levelLoader;
    public PauseMenu pauseMenu;
    public DisconnectedGameMenu disconnectedGameMenu;
    public GameEndMenu gameEndMenu;
    public bool connected;
    private ConnectionState state;
    public bool gameEnd;
    public bool gameEnded;

    void Start()
    {
        levelLoader.LoadLevel(Hypervisor.Default != null ? Hypervisor.Default.levelName ?? "level0" : "level0");
        lings = GameObject.FindObjectOfType<LiNGSClientManager>();
        lings.OnDisconnected += lings_OnDisconnected;
        lings.OnConnectionAccepted += lings_OnConnectionAccepted;
        lings.OnUnableToConnect += lings_OnUnableToConnect;
        lings.OnConnectionRefused += lings_OnConnectionRefused;
        lings.OnReceiveGameMessage += lings_OnReceiveGameMessage;
        connected = true;
    }

    void lings_OnConnectionRefused(LiNGS.Common.Network.NetworkMessage message)
    {
        disconnectedGameMenu.connectionProblem = message.Data[0].Value;
        state = ConnectionState.Failed;
    }

    void lings_OnUnableToConnect(string reason)
    {
        disconnectedGameMenu.connectionProblem = reason;
        state = ConnectionState.Failed;
    }

    void lings_OnConnectionAccepted(LiNGS.Common.Network.NetworkMessage message, bool usingSavedState)
    {
        connected = true;
        state = ConnectionState.Connected;
    }

    void lings_OnDisconnected(string reason)
    {
        disconnectedGameMenu.connectionProblem = reason;
        state = ConnectionState.Disconnected;
    }

    void lings_OnReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        if (message.Data[0].Object == "GameEnd")
        {
            gameEndMenu.winner = message.Data[0].Value.Split(':')[0];
            gameEndMenu.winnerKills = message.Data[0].Value.Split(':')[1];
            gameEnd = true;
        }
    }

    void Update()
    {
        if (gameEnded)
        {
            return;
        }

        if (gameEnd)
        {
            gameEnded = true;
            lings.Client.Disconnect();
            gameEndMenu.GameEnded();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!disconnectedGameMenu.gameObject.activeSelf && !gameEndMenu.gameObject.activeSelf)
            {
                pauseMenu.gameObject.SetActive(true);
            }
        }

        switch (state)
        {
            case ConnectionState.Connected:
                disconnectedGameMenu.ConnectionRestored();
                break;
            case ConnectionState.Disconnected:
                connected = false;
                if (pauseMenu.gameObject.activeSelf)
                {
                    pauseMenu.gameObject.SetActive(false);
                }
                disconnectedGameMenu.gameObject.SetActive(true);
                break;
            case ConnectionState.Failed:
                disconnectedGameMenu.FailedConnection();
                break;
        }
        state = ConnectionState.None;
    }
}
