using UnityEngine;
using System.Collections;

public abstract class Element : MonoBehaviour
{
    public bool canWalk;
    public bool transparent;
    public bool emitsLight;
    public bool isSpawnLocation;
    public bool isItemSpawner;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }
}
