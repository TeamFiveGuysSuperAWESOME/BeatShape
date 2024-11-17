using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Easing
{
    public static float Ease(float t, string easing)
    {
        switch (easing.ToLower())
        {
            case "linear":
                return t;
            case "insine":
                return InSine(t);
            case "outsine":
                return OutSine(t);
            case "incubic":
                return InCubic(t);
            case "outcubic":
                return OutCubic(t);
            case "inoutcubic":
                return InOutCubic(t);
            case "outquint":
                return OutQuint(t);
            default:
                return t;
        }
    }

    // Sine
    public static float InSine(float t)
    {
        return 1 - Mathf.Cos(t * Mathf.PI / 2);
    }
    public static float OutSine(float t)
    {
        return Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI * 0.5f);
    }

    //Cubic
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

    // Quint
    public static float OutQuint(float t)
    {
        return 1 - Mathf.Pow(1 - Mathf.Clamp01(t), 5);
    }

}
