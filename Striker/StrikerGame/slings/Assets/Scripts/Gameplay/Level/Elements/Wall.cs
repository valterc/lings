using UnityEngine;
using System.Collections;

public class Wall : Element
{

    public override void Start()
    {
        canWalk = false;
        transparent = false;
        emitsLight = false;
    }

    public override void Update()
    {

    }
}
