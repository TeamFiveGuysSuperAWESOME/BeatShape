using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScenes : MonoBehaviour
{
    MenuManager manager;
    CanvasGroup canvasGroup;
    public GameObject levelSelect;

    public Vector3 targetPos;
    public Vector3 levelSelect_targetPos;
    public int mapNumber = 1;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Alpha(float a)
    {
        canvasGroup.alpha = a;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 6f);
        levelSelect.transform.localPosition = Vector3.Lerp(levelSelect.transform.localPosition, levelSelect_targetPos, Time.deltaTime * 6f);
        
        if(manager.menuState == "stageSelect")
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(mapNumber < manager.maxLevelIndex) {
                        mapNumber += 1;
                        levelSelect_targetPos -= new Vector3(200, 0, 0);
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                if(manager.sceneState == 0) { // LevelSelect
                    if(mapNumber > 1) {
                        mapNumber -= 1;
                        levelSelect_targetPos += new Vector3(200, 0, 0);
                    }
                }
            }
        }
        
    }
}
