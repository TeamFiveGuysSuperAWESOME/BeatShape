using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    MenuManager manager;
    RectTransform rt;
    MenuScenes ms;
    MenuBeatBoard mbb;

    public KeyCode key;
    public Vector2 scale;
    bool input = false;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        rt = GetComponent<RectTransform>();
        ms = GameObject.FindWithTag("scene").GetComponent<MenuScenes>();
        mbb = GameObject.FindWithTag("beatboard").GetComponent<MenuBeatBoard>();
    }

    void Start()
    {
        RawImage[] rawImages = GetComponentsInChildren<RawImage>();
        foreach (RawImage rawImage in rawImages)
        {
            rawImage.color = manager.menuColor_dark;
        }
    }

    void Update()
    {
        if(manager.menuState == "stageSelect" && (manager.sceneState == 0 || key == KeyCode.UpArrow || key == KeyCode.DownArrow)) {
            if(Input.GetKeyDown(key)) {
                input = true;
                if(key == KeyCode.RightArrow) {ms.Input_right();}
                if(key == KeyCode.LeftArrow) {ms.Input_left();}
                if(key == KeyCode.UpArrow) {mbb.Input_up();}
                if(key == KeyCode.DownArrow) {mbb.Input_down();}
            }
            if(Input.GetKeyUp(key)) {input = false;}
        }
        else {input = false;}

        if(input) {rt.localScale = new Vector3(scale.x*0.9f,scale.y*0.9f,1);}
        else {rt.localScale = new Vector3(scale.x,scale.y,1);}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(manager.menuState == "stageSelect" && (manager.sceneState == 0 || key == KeyCode.UpArrow || key == KeyCode.DownArrow)) {
            input = true;
            if(key == KeyCode.RightArrow) {ms.Input_right();}
            if(key == KeyCode.LeftArrow) {ms.Input_left();}
            if(key == KeyCode.UpArrow) {mbb.Input_up();}
            if(key == KeyCode.DownArrow) {mbb.Input_down();}
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(manager.menuState == "stageSelect" && (manager.sceneState == 0 || key == KeyCode.UpArrow || key == KeyCode.DownArrow)) {
            input = false;
        }
    }
}
