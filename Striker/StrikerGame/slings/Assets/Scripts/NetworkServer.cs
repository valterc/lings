using UnityEngine;
using System.Collections;
using LiNGS.Server;
using LiNGS.Server.GameLogic;
using Striker;
using System.Threading;

public class NetworkServer : MonoBehaviour
{
    private static NetworkServer instance;

    private Thread serverThread;
    private bool serverShutdown;
    private bool startGame;

    public StrikerServer strikerServer;
    
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }

    public void StartServer()
    {
        strikerServer = new StrikerServer("level0", Resources.Load("Levels/level0").ToString(), Resources.Load("names").ToString());
        serverThread = new Thread(ServerUpdate);
        serverThread.Start();
    }

    public void StartGame()
    {
        startGame = true;
    }

    public void Shutdown()
    {
        serverShutdown = true;
    }

    private void ServerUpdate()
    {
        while (!serverShutdown)
        {
            strikerServer.Update();
            if (startGame)
            {
                startGame = false;
                strikerServer.StartGame();
            }

            Thread.Sleep(10);
        }

        if (strikerServer != null)
        {
            try
            {
                strikerServer.Shutdown();
            }
            catch (System.Exception)
            {
            }
        }

        strikerServer = null;
        serverShutdown = false;
    }

}
