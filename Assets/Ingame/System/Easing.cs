using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Easing
{
    public static float InSine(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }
}
