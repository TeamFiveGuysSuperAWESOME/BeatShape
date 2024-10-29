using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClack : MonoBehaviour
{
    SpriteRenderer sr;

    Color startColor;
    Color deltaColor;
    Color endColor;
    float startPos;
    Vector3 startScale;
    float timer;

    void Awake()
    {
        MenuManager manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        sr = GetComponent<SpriteRenderer>();

        endColor = manager.menuColor_dark;
        startColor = new Color(endColor.r+(1f-endColor.r)*0.5f,endColor.g+(1f-endColor.g)*0.5f,endColor.b+(1f-endColor.b)*0.5f,1f);
        deltaColor = new Color(endColor.r-startColor.r,endColor.g-startColor.g,endColor.b-startColor.b,1f);
        startPos = transform.localPosition.y;
        startScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.75f) {Destroy(gameObject);}
        
        transform.localPosition = new Vector2(transform.localPosition.x, startPos + 0.1f*Easing.OutCubic(timer/0.75f));
        transform.localScale = new Vector2(startScale.x + 0.1f*Easing.OutCubic(timer/0.75f), startScale.y*(1-Easing.OutCubic(timer/0.75f)));
        sr.color = new Color(startColor.r+deltaColor.r*(timer/0.75f),startColor.g+deltaColor.g*(timer/0.75f),startColor.b+deltaColor.b*(timer/0.75f),1f);
    }
}
