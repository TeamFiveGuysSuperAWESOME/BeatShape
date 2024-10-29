using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    SpriteRenderer flat_sr;
    SpriteRenderer bb_sr; //beatboard spriteRenderer
    SpriteRenderer bbi_sr; //beatboard_inner spriteRenderer
    SpriteRenderer b_sr; //beat spriteRenderer
    SpriteRenderer bi_sr; //beat_inner spriteRenderer
    TextMeshProUGUI title_tmp;
    TextMeshProUGUI start_tmp;

    public Color menuColor;
    public Color menuColor_light;
    public Color menuColor_dark;
    
    public string menuState = "menu";

    void Awake()
    {
        flat_sr = GameObject.FindWithTag("flat").GetComponent<SpriteRenderer>();
        bb_sr = GameObject.FindWithTag("beatboard").GetComponent<SpriteRenderer>();
        bbi_sr = GameObject.FindWithTag("beatboard_inner").GetComponent<SpriteRenderer>();
        title_tmp = GameObject.FindWithTag("title_text").GetComponent<TextMeshProUGUI>();
        start_tmp = GameObject.FindWithTag("start_text").GetComponent<TextMeshProUGUI>();
        
        menuColor = new Color(Random.Range(0.7f,1f), Random.Range(0.7f,1f), Random.Range(0.7f,1f), 1f);
        menuColor_light = new Color(0.5f+menuColor.r*0.5f, 0.5f+menuColor.g*0.5f, 0.5f+menuColor.b*0.5f, 1f);
        menuColor_dark = new Color(menuColor.r*0.3f, menuColor.g*0.3f, menuColor.b*0.3f, 1f);

        flat_sr.color = menuColor;
        bbi_sr.color = menuColor_light;
        bb_sr.color = menuColor_dark;
        title_tmp.color = menuColor_light;
        title_tmp.outlineColor = menuColor_dark;
        start_tmp.color = menuColor_dark;

    }

    void Update()
    {
        
    }
}
