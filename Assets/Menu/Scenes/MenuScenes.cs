using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuScenes : MonoBehaviour
{
    MenuManager manager;
    CanvasGroup canvasGroup;
    LevelSelectScene levelSelect;
    TextMeshProUGUI stage_tmp;

    public Vector3 targetPos;
    public Vector3 liveLevelScale;

    public GameObject arrows_LR;
    GameObject stage_tmp_obj;

    float timer = 0f;
    float timer_stageEntry = 0f;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        levelSelect = GetComponentInChildren<LevelSelectScene>();
        canvasGroup = GetComponent<CanvasGroup>();
        stage_tmp_obj = GameObject.FindWithTag("stage_text");
        stage_tmp = stage_tmp_obj.GetComponent<TextMeshProUGUI>();
    }

    public void Alpha(float a)
    {
        canvasGroup.alpha = a;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 6f);
        
        if(manager.menuState == "stageSelect")
        {
            timer += Time.deltaTime;
            liveLevelScale = new Vector3(0.5f*Mathf.Sin(timer)+10f, 0.5f*Mathf.Sin(timer)+10f, 1);

            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(manager.levelNumber < manager.levelIndex) {
                        levelSelect.levels[manager.levelNumber-1].GetComponent<MenuLevel>().targetScale = new Vector3(5, 5, 1);
                        manager.levelNumber += 1;
                        levelSelect.targetPos -= new Vector3(10, 0, 0);
                        stage_tmp.text = "STAGE " + manager.levelNumber;
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(manager.levelNumber > 1) {
                        levelSelect.levels[manager.levelNumber-1].GetComponent<MenuLevel>().targetScale = new Vector3(5, 5, 1);
                        manager.levelNumber -= 1;
                        levelSelect.targetPos += new Vector3(10, 0, 0);
                        stage_tmp.text = "STAGE " + manager.levelNumber;
                    }
                }
            }
            levelSelect.levels[manager.levelNumber-1].GetComponent<MenuLevel>().targetScale = liveLevelScale;
        }
        
        if(manager.menuState == "stageEntry") {
            if(timer_stageEntry < 1f) {
                timer_stageEntry += Time.deltaTime;

                levelSelect.levels[manager.levelNumber-1].GetComponent<MenuLevel>().targetScale = new Vector3(13, 13, 1);
                if(manager.levelNumber-2 >= 0) levelSelect.levels[manager.levelNumber-2].transform.localPosition = new Vector3(10*(manager.levelNumber-2) - 5.1f*Easing.OutQuint(timer_stageEntry/1f),0,0);
                if(manager.levelNumber <= manager.levelIndex-1) levelSelect.levels[manager.levelNumber].transform.localPosition = new Vector3(10*(manager.levelNumber) + 5.1f*Easing.OutQuint(timer_stageEntry/1f),0,0);

                arrows_LR.transform.localPosition = new Vector3(arrows_LR.transform.localPosition.x, -15f*Easing.OutQuint(timer_stageEntry/1f), 0);
                stage_tmp_obj.transform.localPosition = new Vector3(stage_tmp_obj.transform.localPosition.x, -7 - 1.5f*Easing.OutQuint(timer_stageEntry/1f), 0);
            }
            else {timer_stageEntry = 1f;}
            
        }
    }
}
