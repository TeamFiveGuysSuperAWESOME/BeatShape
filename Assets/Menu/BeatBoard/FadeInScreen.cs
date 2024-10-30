using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScreen : MonoBehaviour
{
    SpriteRenderer sr;

    float opacity = 1f;
    Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        color.a = opacity;
        sr.color = color;
        if(opacity > 0f) {
            opacity -= 1.5f*Time.deltaTime;
        }
        else {
            opacity = 0f;
        }
    }
}
