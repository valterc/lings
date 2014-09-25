using UnityEngine;
using System.Collections;

public interface IObjectManager 
{
     LiNGS.Common.GameLogic.INetworkedObject CreateObject(string typeName, string name);
     void DestroyObject(LiNGS.Common.GameLogic.INetworkedObject networkedObject, string name);
}
