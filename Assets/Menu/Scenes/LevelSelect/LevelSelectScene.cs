using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScene : MonoBehaviour
{
    MenuManager manager;
    MenuEffects effects;
    MenuScenes menuScenes;
    FadeInScreen screen;
    AudioSource audioSource;

    public GameObject level_prefab;
    public GameObject[] levels;
    GameObject levelsGroup;
    public GameObject stage_text_obj;
    RectTransform stage_text_rt;

    public Vector3 targetPos;
    float timer = 0f;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        effects = GameObject.FindWithTag("effectmanager").GetComponent<MenuEffects>();
        menuScenes = GetComponentInParent<MenuScenes>();
        screen = GameObject.FindWithTag("screen").GetComponent<FadeInScreen>();
        audioSource = GetComponent<AudioSource>();

        levelsGroup = GameObject.FindWithTag("levels");
        stage_text_rt = stage_text_obj.GetComponent<RectTransform>();
    }

    void Start()
    {
        levels = new GameObject[manager.levelIndex];
        for(int i=0; i<manager.levelIndex; i++) {
            levels[i] = Instantiate(level_prefab, GameObject.FindWithTag("levels").transform);
            levels[i].transform.localPosition += new Vector3(10*i, 0, 0);

            MenuLevel level_scr = levels[i].GetComponent<MenuLevel>();
            if(i == MenuManager.levelNumber-1) {level_scr.targetScale = new Vector3(10, 10, 1);}
            else {level_scr.targetScale = new Vector3(5, 5, 1);}
            level_scr.stageNum = i;
        }
    }

    public void Input_spaceDown()
    {
        if(manager.menuState == "stageSelect" && manager.sceneState == 0) {
            stage_text_rt.localScale = new Vector3(0.9f,0.9f,1);
        }
    }
    public void Input_spaceUp()
    {
        if(manager.menuState == "stageSelect" && manager.sceneState == 0) {
            stage_text_rt.localScale = new Vector3(1,1,1);
            var tempLevel = Resources.Load<TextAsset>("Levels/" + MenuManager.levelNumber + "/level");
            if (MenuManager.levelNumber == 1) {
                if (tempLevel != null) 
                {
                    effects.NewSquare(new Vector2(0,0), new Vector2(menuScenes.liveLevelScale.x*25,menuScenes.liveLevelScale.y*25), new Vector2(350,350), 0.75f, manager.menuColor_dark);
                    manager.menuState = "stageEntry";
                    audioSource.volume = MenuSoundManager.sfxVolume;
                    audioSource.Play();
                }
                else Debug.Log("Level not found");
            }
            else Debug.Log("Level not opened");
        }
    }

    void Update()
    {
        levelsGroup.transform.localPosition = Vector3.Lerp(levelsGroup.transform.localPosition, targetPos, Time.deltaTime * 6f);
        
        //input_spaceUp / Down
        if(manager.menuState == "stageSelect" && manager.sceneState == 0) {
            if(Input.GetKeyDown(KeyCode.Space)) 
            {
                Input_spaceDown();
            }
            if(Input.GetKeyUp(KeyCode.Space)) 
            {
                Input_spaceUp();
            }
        }
        else {stage_text_rt.localScale = new Vector3(1,1,1);}

        if(manager.menuState == "stageEntry") {
            if(timer >= 0f) timer += Time.deltaTime;
            
            if(timer > 1f) 
            {
                screen.screenState = "FadeIn";
                timer = -1f;
            }
        }
    }
}
