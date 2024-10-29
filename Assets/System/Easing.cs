using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Easing
{
    public static float InSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }

    public static float InCubic(float t)
    {
        return t * t * t;
    }

    public static float OutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    public static float InOutCubic(float t)
    {
        if (t < 0.5f)
        {
            return 4 * t * t * t;
        }
        else
        {
            return 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
        }
    }
}
