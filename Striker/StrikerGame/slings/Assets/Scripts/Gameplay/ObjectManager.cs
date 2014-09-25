using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiNGS.Common.GameLogic;

public class ObjectManager : MonoBehaviour, IObjectManager
{
    private Dictionary<string, GameObject> objectTypeCache;
    private LiNGSClientManager lings;

    public GameObject CharactersParent;
    public GameObject NPCPrefab;
    public GameObject PlayerPrefab;
    public GameObject BulletPrefab;

    void Start()
    {
        objectTypeCache = new Dictionary<string, GameObject>() 
        { 
            { "Striker.Characters.NPC", NPCPrefab },
            { "Striker.Characters.Player", NPCPrefab },
            { "Striker.Characters.CPlayer", PlayerPrefab },
            { "Striker.Elements.Bullet", BulletPrefab }
        };

        lings = GameObject.FindObjectOfType<LiNGSClientManager>();
        if (lings != null)
        {
            lings.ObjectManager = this;
        }
    }

    void Update()
    {

    }

    #region IObjectManager Members

    public LiNGS.Common.GameLogic.INetworkedObject CreateObject(string typeName, string name)
    {
        GameObject go = (GameObject)GameObject.Instantiate(objectTypeCache[typeName]);
        go.transform.parent = CharactersParent.transform;
        go.name = name;
        return go.GetComponent(typeof(INetworkedObject)) as INetworkedObject;
    }

    public void DestroyObject(LiNGS.Common.GameLogic.INetworkedObject networkedObject, string name)
    {
        Destroy(GameObject.Find(name));
    }

    #endregion
}
