using UnityEngine;
using System.Collections;
using LiNGS.Client.GameLogic;
using LiNGS.Client;

public class NetworkClient : MonoBehaviour
{
    private static NetworkClient instance;


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

    public void Connect(string ipAddress)
    {
       
    }

}
