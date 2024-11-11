using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBeat : MonoBehaviour
{
    MenuManager manager;
    public MenuEffects em;

    bool isPlaying = false;
    float height;
    float size;
    float time;
    float time_temp;
    int order;
    float startPos;
    float targetPos;

    void Start()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        em = GameObject.FindWithTag("effectmanager").GetComponent<MenuEffects>();

        GetComponentsInChildren<SpriteRenderer>()[0].color = manager.menuColor_dark;
        GetComponentsInChildren<SpriteRenderer>()[1].color = manager.menuColor_light;
    }

    public void BeatSetting(int o, float s, float h, float t)
    {
        isPlaying = true;
        order = o;
        size = s;
        height = h;
        time = t;
        time_temp = time;
        startPos = transform.localPosition.y;
        targetPos = startPos + height;
    }

    void Update()
    {
        if(isPlaying)
        {
            if(time <= 0) {
                MenuBeatBoard bb = GetComponentInParent<MenuBeatBoard>();
                bb.NewClack();
                bb.transform.localScale *= 1.075f;

                
                em.NewSquare(new Vector2(0,-110), new Vector2(150,150), new Vector2(300,300), 1, manager.menuColor_dark);

                Destroy(gameObject);
            }

            time -= Time.deltaTime;
            if(time > time_temp/2) {transform.localPosition = new Vector2(transform.localPosition.x, startPos + height*Easing.OutCubic((time_temp-time)/(time_temp/2)));}
            else {transform.localPosition = new Vector2(transform.localPosition.x, targetPos - height*Easing.InCubic((time_temp/2-time)/(time_temp/2)));}
        }
    }
}
