using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameManager;

public class MenuManager : MonoBehaviour
{
    SpriteRenderer flat_sr;
    SpriteRenderer bb_sr; //beatboard spriteRenderer
    SpriteRenderer bbi_sr; //beatboard_inner spriteRenderer
    SpriteRenderer b_sr; //beat spriteRenderer
    SpriteRenderer bi_sr; //beat_inner spriteRenderer
    TextMeshProUGUI title_tmp;
    TextMeshProUGUI start_tmp;
    TextMeshProUGUI stage_tmp;

    public Color menuColor;
    public Color menuColor_light;
    public Color menuColor_dark;
    
    public string menuState = "menu";
    public int sceneState = 0;
    public int levelIndex = 5;
    public static int levelNumber = 2;

    private readonly KeyCode[] cheatCode = { KeyCode.D, KeyCode.G, KeyCode.B, KeyCode.A, KeyCode.B, KeyCode.O };
    public static bool DebugMode = false;
    private int codeIndex = 0;
    private float timer = 0f;
    private readonly float timeLimit = 10f;
    

    void Awake()
    {
        flat_sr = GameObject.FindWithTag("flat").GetComponent<SpriteRenderer>();
        bb_sr = GameObject.FindWithTag("beatboard").GetComponent<SpriteRenderer>();
        bbi_sr = GameObject.FindWithTag("beatboard_inner").GetComponent<SpriteRenderer>();
        title_tmp = GameObject.FindWithTag("title_text").GetComponent<TextMeshProUGUI>();
        start_tmp = GameObject.FindWithTag("start_text").GetComponent<TextMeshProUGUI>();
        stage_tmp = GameObject.FindWithTag("stage_text").GetComponent<TextMeshProUGUI>();
        
        menuColor = new Color(Random.Range(0.7f,1f), Random.Range(0.7f,1f), Random.Range(0.7f,1f), 1f);
        menuColor_light = new Color(0.5f+menuColor.r*0.5f, 0.5f+menuColor.g*0.5f, 0.5f+menuColor.b*0.5f, 1f);
        menuColor_dark = new Color(menuColor.r*0.3f, menuColor.g*0.3f, menuColor.b*0.3f, 1f);

        flat_sr.color = menuColor;
        bbi_sr.color = menuColor_light;
        bb_sr.color = menuColor_dark;
        title_tmp.color = menuColor_light;
        title_tmp.outlineColor = menuColor_dark;
        start_tmp.color = menuColor_dark;
        stage_tmp.color = menuColor_dark;
    }

    void Update()
    {
        if (codeIndex > 0)
        {
            timer += Time.deltaTime;
            if (timer > timeLimit)
            {
                codeIndex = 0;
                timer = 0f;
            }
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(cheatCode[codeIndex]))
            {
                codeIndex++;
                if (codeIndex == cheatCode.Length)
                {
                    Debug.Log("Debug mode toggled!");
                    DebugMode = !DebugMode;
                    codeIndex = 0;
                    timer = 0f;
                }
                else if (codeIndex == 1)
                {
                    timer = 0f;
                }
            }
            else
            {
                codeIndex = 0;
                timer = 0f;
            }
        }
    }
}
