using UnityEngine;
using System.Collections;
using LiNGS.Client.GameLogic;
using LiNGS.Client;

public class LiNGSClientManager : MonoBehaviour, INetworkedClient
{

    public delegate void ConnectionAccepted(LiNGS.Common.Network.NetworkMessage message, bool usingSavedState);
    public delegate void ConnectionRefused(LiNGS.Common.Network.NetworkMessage message);
    public delegate void Disconnected(string reason);
    public delegate void UnableToConnect(string reason);

    public delegate void ReceiveErrorMessage(LiNGS.Common.Network.NetworkMessage message);
    public delegate void ReceiveEventMessage(LiNGS.Common.Network.NetworkMessage message);
    public delegate void ReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message);

    public event ConnectionAccepted OnConnectionAccepted;
    public event ConnectionRefused OnConnectionRefused;
    public event Disconnected OnDisconnected;
    public event UnableToConnect OnUnableToConnect;

    public event ReceiveErrorMessage OnReceiveErrorMessage;
    public event ReceiveEventMessage OnReceiveEventMessage;
    public event ReceiveGameMessage OnReceiveGameMessage;

    private static LiNGSClientManager instance;
    private LiNGSClient lingsClient;

    public LiNGSClient Client
    {
        get
        {
            return lingsClient;
        }
    }

    public IObjectManager ObjectManager;

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
        if (lingsClient != null)
        {
            lingsClient.Update();
        }
    }

    void OnDestroy()
    {
        if (lingsClient != null)
        {
            lingsClient.Disconnect();
        }
    }

    public void Connect(ClientProperties clientProperties, ServerInfo serverInfo)
    {
        lingsClient = new LiNGSClient(clientProperties, serverInfo, this);
        lingsClient.Connect();
    }

    #region INetworkedClient Members

    void INetworkedClient.ConnectionAccepted(LiNGS.Common.Network.NetworkMessage message, bool usingSavedState)
    {
        if (OnConnectionAccepted != null)
        {
            OnConnectionAccepted(message, usingSavedState);
        }
    }

    void INetworkedClient.ConnectionRefused(LiNGS.Common.Network.NetworkMessage message)
    {
        if (OnConnectionRefused != null)
        {
            OnConnectionRefused(message);
        }
    }

    LiNGS.Common.GameLogic.INetworkedObject INetworkedClient.CreateObject(string typeName, string name)
    {
        return ObjectManager.CreateObject(typeName, name);
    }

    void INetworkedClient.DestroyObject(LiNGS.Common.GameLogic.INetworkedObject networkedObject, string name)
    {
        ObjectManager.DestroyObject(networkedObject, name);
    }

    void INetworkedClient.Disconnected(string reason)
    {
        if (OnDisconnected != null)
        {
            OnDisconnected(reason);
        }
    }

    void INetworkedClient.ReceiveErrorMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        if (OnReceiveErrorMessage != null)
        {
            OnReceiveErrorMessage(message);
        }
    }

    void INetworkedClient.ReceiveEventMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        if (OnReceiveEventMessage != null)
        {
            OnReceiveEventMessage(message);
        }
    }

    void INetworkedClient.ReceiveGameMessage(LiNGS.Common.Network.NetworkMessage message)
    {
        if (OnReceiveGameMessage != null)
        {
            OnReceiveGameMessage(message);
        }
    }

    void INetworkedClient.UnableToConnect(string reason)
    {
        if (OnUnableToConnect != null)
        {
            OnUnableToConnect(reason);
        }
    }

    #endregion

}
