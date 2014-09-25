using UnityEngine;
using System.Collections;

public class Floor : Element
{

    public override void Start()
    {
        canWalk = true;
        transparent = true;
        emitsLight = false;
    }

    public override void Update()
    {

    }
}
