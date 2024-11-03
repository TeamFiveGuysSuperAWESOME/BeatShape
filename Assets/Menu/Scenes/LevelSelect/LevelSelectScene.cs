using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScene : MonoBehaviour
{
    MenuManager manager;
    MenuScenes menuScenes;

    public GameObject level_prefab;
    public GameObject[] levels;
    GameObject levelsGroup;

    public Vector3 targetPos;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        menuScenes = GetComponentInParent<MenuScenes>();
        levelsGroup = GameObject.FindWithTag("levels");
    }

    void Start()
    {
        levels = new GameObject[manager.levelIndex];
        for(int i=0; i<manager.levelIndex; i++) {
            levels[i] = Instantiate(level_prefab, GameObject.FindWithTag("levels").transform);
            levels[i].transform.localPosition += new Vector3(10*i, 0, 0);
            if(i == menuScenes.levelNumber-1) {levels[i].GetComponent<MenuLevel>().targetScale = new Vector3(10, 10, 1);}
            else {levels[i].GetComponent<MenuLevel>().targetScale = new Vector3(5, 5, 1);}
        }
    }

    void Update()
    {
        levelsGroup.transform.localPosition = Vector3.Lerp(levelsGroup.transform.localPosition, targetPos, Time.deltaTime * 6f);
    }
}
