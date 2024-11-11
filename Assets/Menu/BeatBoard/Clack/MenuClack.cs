using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClack : MonoBehaviour
{
    MenuManager manager;
    SpriteRenderer sr;
    public GameObject mask1;
    public GameObject mask2;

    Color startColor;
    Color deltaColor;
    Color endColor;
    float startPos;
    Vector3 startScale;
    float time = 0.5f;
    float timer;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
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
        if(timer > time) {
            Destroy(gameObject);
        }
        
        mask1.transform.localPosition = new Vector2(0, Easing.OutQuint(timer/time));
        mask2.transform.localPosition = new Vector2(0, -1+Easing.OutSine(timer/time));
        
        sr.color = new Color(startColor.r+deltaColor.r*(timer/time),startColor.g+deltaColor.g*(timer/time),startColor.b+deltaColor.b*(timer/time),1f);
    }
}
