using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameManager;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    FadeInScreen screen;
    public GameObject offsetSettings_obj, frameSettings_obj, vsyncSettings_obj;
    TextMeshProUGUI offsetSettings, frameSettings, vsyncSettings;

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
    private bool isStageEnter = false;
    private readonly float timeLimit = 10f;
    

    void Awake()
    {
        screen = GameObject.FindWithTag("screen").GetComponent<FadeInScreen>();
        offsetSettings = offsetSettings_obj.GetComponent<TextMeshProUGUI>();
        frameSettings = frameSettings_obj.GetComponent<TextMeshProUGUI>();
        vsyncSettings = vsyncSettings_obj.GetComponent<TextMeshProUGUI>();

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

        if(!PlayerPrefs.HasKey("frame") || PlayerPrefs.GetInt("frame")%15 != 0) PlayerPrefs.SetInt("frame", 60);
        if(!PlayerPrefs.HasKey("vsync")) PlayerPrefs.SetInt("vsync", 1);
        offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
        frameSettings.text = PlayerPrefs.GetInt("frame").ToString();
        if(PlayerPrefs.GetInt("vsync")!=0) {
            vsyncSettings.text = "(Vsync)\nOn";
            vsyncSettings.color = new Color32(70,210,70,255);
            frameSettings_obj.SetActive(false);
        }
        else {
            vsyncSettings.text = "(Vsync)\nOff";
            vsyncSettings.color = new Color32(100,100,100,255);
            frameSettings_obj.SetActive(true);
        }
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync");
        Application.targetFrameRate = PlayerPrefs.GetInt("frame");
    }

    public void ResetOffset() 
    {
        PlayerPrefs.SetFloat("calibratedOffset", 0f);
        PlayerPrefs.SetInt("isCalibrated", 0);
        timer = 0;
        
        screen.screenState = "FadeIn";
        isStageEnter = true;
    }

    public void UpValue(string type)
    {
        switch (type) {
            case "offset":
                PlayerPrefs.SetFloat("calibratedOffset", PlayerPrefs.GetFloat("calibratedOffset") + 0.01f);
                offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
                break;
            case "frame":
                if(PlayerPrefs.GetInt("frame") < 90) {
                    PlayerPrefs.SetInt("frame", PlayerPrefs.GetInt("frame") + 15);
                    frameSettings.text = PlayerPrefs.GetInt("frame").ToString();
                    Application.targetFrameRate = PlayerPrefs.GetInt("frame");
                }
                break;
        }
    }
    public void DownValue(string type)
    {
        switch (type) {
            case "offset":
                PlayerPrefs.SetFloat("calibratedOffset", PlayerPrefs.GetFloat("calibratedOffset") - 0.01f);
                offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
                break;
            case "frame":
                if(PlayerPrefs.GetInt("frame") > 15) {
                    PlayerPrefs.SetInt("frame", PlayerPrefs.GetInt("frame") - 15);
                    frameSettings.text = PlayerPrefs.GetInt("frame").ToString();
                    Application.targetFrameRate = PlayerPrefs.GetInt("frame");
                }
                break;
        }
    }
    public void Toggle(string type)
    {
        switch (type) {
            case "vsync":
                if(PlayerPrefs.GetInt("vsync") == 0) {
                    PlayerPrefs.SetInt("vsync", 1);
                    frameSettings_obj.SetActive(false);
                    vsyncSettings.text = "(Vsync)\nOn";
                    vsyncSettings.color = new Color32(70,210,70,255);
                }
                else {
                    PlayerPrefs.SetInt("vsync", 0);
                    frameSettings_obj.SetActive(true);
                    vsyncSettings.text = "(Vsync)\nOff";
                    vsyncSettings.color = new Color32(100,100,100,255);
                }
                QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync");
                Debug.Log(PlayerPrefs.GetInt("vsync"));
                break;
        }
    }

    void Update()
    {
        if(isStageEnter) {
            timer += Time.deltaTime;
            if(timer > 1f) {SceneManager.LoadScene("InGame");}
        }

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
