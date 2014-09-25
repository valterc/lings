using UnityEngine;
using System.Collections;
using LiNGS.Client.GameLogic;
using LiNGS.Client;
using LiNGS.Common.GameLogic;

public class OLD_NetworkClient : MonoBehaviour, INetworkedClient {

	public GameObject objectPrefab;

    public static OLD_NetworkClient Default { get; private set; }
	private LiNGSClient networkClient;

	void Start () 
	{
		if (Default != null) 
		{
			Destroy(gameObject);
		}
		
		Default = this;
		DontDestroyOnLoad(gameObject);

        ClientProperties properties = ClientProperties.Default;
        ServerInfo serverInfo = ServerInfo.DefaultLocal;

        networkClient = new LiNGSClient(properties, serverInfo, this);
		networkClient.Connect();
	}
	
	void Update () 
	{
		networkClient.Update();
	}

	void OnApplicationQuit()
	{
		networkClient.Disconnect();
	}

	#region INetworkedClient implementation

    public void ConnectionAccepted(LiNGS.Common.Network.NetworkMessage message, bool usingSavedState)
    {
        Debug.Log("Connected");
    }

    public void Disconnected(string reason)
    {
        Debug.Log("Disconnected -- " + reason);
    }

    public void UnableToConnect(string reason)
    {
        Debug.Log("Unable to connect -- " + reason);
    }

	public void ConnectionRefused (LiNGS.Common.Network.NetworkMessage message)
	{
	
	}

	public void ReceiveGameMessage (LiNGS.Common.Network.NetworkMessage message)
	{
	
	}

	public void ReceiveEventMessage (LiNGS.Common.Network.NetworkMessage message)
	{
	
	}

	public void ReceiveErrorMessage (LiNGS.Common.Network.NetworkMessage message)
	{
	
	}

	public LiNGS.Common.GameLogic.INetworkedObject CreateObject (string typeName, string name)
	{
		return BallGameObject.Instantiate(name, objectPrefab);
	}

	public void DestroyObject (LiNGS.Common.GameLogic.INetworkedObject networkedObject, string name)
	{
		GameObject go = GameObject.Find(name);
		if (go != null) 
		{
			Destroy(go);
		}
	}

	#endregion

}
