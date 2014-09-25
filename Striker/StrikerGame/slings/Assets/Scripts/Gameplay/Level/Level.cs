using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level 
{

    public List<Element> map;
    public int Width;
    public int Height;

    public Level()
    {
        map = new List<Element>();
    }

}
