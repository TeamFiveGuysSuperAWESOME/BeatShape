using UnityEngine;
using TMPro;

public class Bar : MonoBehaviour
{
    MenuManager manager;

    bool onClick = false;
    public string type = "Music";
    
    public GameObject bar_front;
    LineRenderer lr;
    public GameObject text;
    TextMeshProUGUI text_tmp;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        lr = bar_front.GetComponent<LineRenderer>();
        text_tmp = text.GetComponent<TextMeshProUGUI>();
    }

    void OnMouseDown()
    {
        onClick = true;
    }
    void OnMouseUp()
    {
        onClick = false;
    }

    void Update()
    {
        if(manager.sceneState == 1) {
            if(onClick) {
                Vector3 newPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,0,0);
                newPos = new Vector3(transform.InverseTransformPoint(newPos).x + 0.5f,0,0);
                if(newPos.x < 0) {lr.SetPosition(1, new Vector3(0,0,0));}
                else if(newPos.x > 1) {lr.SetPosition(1, new Vector3(10,0,0));}
                else {lr.SetPosition(1, newPos*10);}
                text_tmp.text = (lr.GetPosition(1).x*10).ToString("F0") + "%";
                
                if(type == "Music") {MenuSoundManager.musicVolume = lr.GetPosition(1).x/10;}
                else if(type == "SoundEffect") {MenuSoundManager.sfxVolume = lr.GetPosition(1).x/10;}
            }
        }
    }
}
