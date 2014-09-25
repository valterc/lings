using UnityEngine;
using System.Collections;

public static class ScaledValue { 

    public static int Scaled(this int value)
    {
        return (value * Screen.width) / 872;
    }

    public static float Scaled(this float value)
    {
        return (value * Screen.width) / 872;
    }

    public static double Scaled(this double value)
    {
        return (value * Screen.width) / 872;
    }
}
