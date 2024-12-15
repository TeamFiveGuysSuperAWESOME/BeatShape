using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameManager;

public class MenuManager : MonoBehaviour
{
    public Color menuColor;
    public Color menuColor_light;
    public Color menuColor_dark;
    
    public string menuState = "menu";
    public int sceneState = 0;
    public int levelIndex = 5;
    public static int levelNumber = 1;

    private readonly KeyCode[] cheatCode = { KeyCode.D, KeyCode.G, KeyCode.B, KeyCode.A, KeyCode.B, KeyCode.O };
    public static bool DebugMode = false;
    private int codeIndex = 0;
    private float timer = 0f;
    private readonly float timeLimit = 10f;
    

    void Awake()
    {
        menuColor = new Color(Random.Range(0.7f,1f), Random.Range(0.7f,1f), Random.Range(0.7f,1f), 1f);
        menuColor_light = new Color(0.5f+menuColor.r*0.5f, 0.5f+menuColor.g*0.5f, 0.5f+menuColor.b*0.5f, 1f);
        menuColor_dark = new Color(menuColor.r*0.3f, menuColor.g*0.3f, menuColor.b*0.3f, 1f);

        GameObject[] credit_title = GameObject.FindGameObjectsWithTag("credit_title");
        GameObject[] credit_text = GameObject.FindGameObjectsWithTag("credit_text");
        foreach(GameObject obj in credit_title) {
            obj.GetComponent<TextMeshProUGUI>().color = menuColor_light;
            obj.GetComponent<TextMeshProUGUI>().fontMaterial.SetColor("_UnderlayColor", menuColor_dark);
        }
        foreach(GameObject obj in credit_text) {
            obj.GetComponent<TextMeshProUGUI>().color = menuColor_light;
            obj.GetComponent<TextMeshProUGUI>().fontMaterial.SetColor("_UnderlayColor", menuColor_dark);
        }

        GameObject.FindWithTag("flat").GetComponent<SpriteRenderer>().color = menuColor;
        GameObject.FindWithTag("beatboard_inner").GetComponent<SpriteRenderer>().color = menuColor_light;
        GameObject.FindWithTag("beatboard").GetComponent<SpriteRenderer>().color = menuColor_dark;
        GameObject.FindWithTag("debug_img").GetComponent<SpriteRenderer>().color = menuColor_light;
        GameObject.FindWithTag("debug_imgShadow").GetComponent<SpriteRenderer>().color = menuColor_dark;
        GameObject.FindWithTag("title_text").GetComponent<TextMeshProUGUI>().color = menuColor_light;
        GameObject.FindWithTag("title_text").GetComponent<TextMeshProUGUI>().fontMaterial.SetColor("_UnderlayColor", menuColor_dark);
        GameObject.FindWithTag("debug_text").GetComponent<TextMeshProUGUI>().color = menuColor_light;
        GameObject.FindWithTag("debug_text").GetComponent<TextMeshProUGUI>().fontMaterial.SetColor("_UnderlayColor", menuColor_dark);
        GameObject.FindWithTag("start_text").GetComponent<TextMeshProUGUI>().color = menuColor_dark;
        GameObject.FindWithTag("stage_text").GetComponent<TextMeshProUGUI>().color = menuColor_dark;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
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
