using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceToStart : MonoBehaviour
{
    TextMeshProUGUI tmp;
    TextMeshProUGUI title_tmp;
    RectTransform rt;
    MenuManager manager;
    MenuBeatBoard beatboard;
    
    float timer;
    bool isHolding;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        title_tmp = GameObject.FindWithTag("title_text").GetComponent<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        beatboard = GameObject.FindWithTag("beatboard").GetComponent<MenuBeatBoard>();
    }

    void Update()
    {
        if(manager.menuState == "menu") {
            timer = 0f;
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f);
            title_tmp.color = new Color(title_tmp.color.r, title_tmp.color.g, title_tmp.color.b, 1f);

            if(Input.GetKeyDown(KeyCode.Space)) {
                isHolding = true;
                rt.localScale = new Vector2(0.85f, 0.85f);
            }
            if(isHolding && Input.GetKeyUp(KeyCode.Space)) {
                isHolding = false;
                manager.menuState = "menuToStageSelect";
                rt.localScale = new Vector2(1f, 1f);
                beatboard.NewBeat();
            }
        }

        if(manager.menuState == "menuToStageSelect") {
            if(timer < 1f) {
                timer += Time.deltaTime;
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f-timer*(1/1f));
                title_tmp.color = new Color(title_tmp.color.r, title_tmp.color.g, title_tmp.color.b, 1f-timer*(1/1f));
            }
        }

    }
}
