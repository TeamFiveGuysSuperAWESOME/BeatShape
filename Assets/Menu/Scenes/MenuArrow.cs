using UnityEngine;
using UnityEngine.UI;

public class MenuArrow : MonoBehaviour
{
    MenuManager manager;
    RectTransform rt;

    public KeyCode key;
    public Vector2 scale;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        rt = GetComponent<RectTransform>();
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
            if(Input.GetKeyDown(key)) {rt.localScale = new Vector3(scale.x*0.9f,scale.y*0.9f,1);}
            if(Input.GetKeyUp(key)) {rt.localScale = new Vector3(scale.x,scale.y,1);}
        }
        else {rt.localScale = new Vector3(scale.x,scale.y,1);}
    }
}
