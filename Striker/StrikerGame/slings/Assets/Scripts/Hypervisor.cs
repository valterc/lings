using UnityEngine;
using System.Collections;

public class Hypervisor : MonoBehaviour
{

    private static Hypervisor instance;
    public static Hypervisor Default
    {
        get
        {
            return instance;
        }
    }
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Game State

    public string levelName;

    #endregion
    

    

}
