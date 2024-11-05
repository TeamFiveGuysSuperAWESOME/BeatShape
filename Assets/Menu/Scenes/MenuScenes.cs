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
    public int levelNumber = 1;

    float timer;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        levelSelect = GetComponentInChildren<LevelSelectScene>();
        canvasGroup = GetComponent<CanvasGroup>();
        stage_tmp = GameObject.FindWithTag("stage_text").GetComponent<TextMeshProUGUI>();
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
            Vector3 liveLevelScale = new Vector3(0.5f*Mathf.Sin(timer)+10f, 0.5f*Mathf.Sin(timer)+10f, 1);

            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(levelNumber < manager.levelIndex) {
                        levelSelect.levels[levelNumber-1].GetComponent<MenuLevel>().targetScale = new Vector3(5, 5, 1);
                        levelNumber += 1;
                        levelSelect.targetPos -= new Vector3(10, 0, 0);
                        stage_tmp.text = "STAGE " + levelNumber;
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(levelNumber > 1) {
                        levelSelect.levels[levelNumber-1].GetComponent<MenuLevel>().targetScale = new Vector3(5, 5, 1);
                        levelNumber -= 1;
                        levelSelect.targetPos += new Vector3(10, 0, 0);
                        stage_tmp.text = "STAGE " + levelNumber;
                    }
                }
            }
            levelSelect.levels[levelNumber-1].GetComponent<MenuLevel>().targetScale = liveLevelScale;
        }
        
    }
}
