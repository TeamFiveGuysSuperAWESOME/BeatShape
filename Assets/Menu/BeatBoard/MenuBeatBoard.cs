using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBeatBoard : MonoBehaviour
{
    MenuManager manager;

    public GameObject beat_prefab;
    public GameObject clack_prefab;

    float rot_z;
    float timer;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
    }

    public void NewBeat()
    {
        GameObject beat_obj = Instantiate(beat_prefab, transform);
        MenuBeat beat_scr = beat_obj.GetComponent<MenuBeat>();
        beat_scr.BeatSetting(0,1f,0.5f,0.8f);
    }

    public void NewClack()
    {
        GameObject clack_obj = Instantiate(clack_prefab, transform);
    }

    void Update()
    {
        if(manager.menuState == "menu") {
            transform.rotation = Quaternion.Euler(0, 0, rot_z);
            rot_z += 10*Time.deltaTime;
            if(rot_z >= 45f) {rot_z -= 90f;}
            timer = 0f;
        }

        if(manager.menuState == "menuToStageSelect") {
            timer += Time.deltaTime;
            if(timer < 0.75f) {
                rot_z = rot_z + (-rot_z)*5*Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, rot_z);
            }
            else if(timer < 2.25f) {
                float inoutCubic = Easing.InOutCubic((timer-0.75f)/1.5f);
                transform.position = new Vector2(-230*inoutCubic, -110+111*inoutCubic);
                rot_z = -180f*inoutCubic;
                transform.rotation = Quaternion.Euler(0, 0, rot_z);
            }
            else {
                manager.menuState = "stageSelect";
            }
        }
        
        if(manager.menuState == "stageSelect") {
            Quaternion targetRotation = Quaternion.Euler(0, 0, rot_z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 6f);
            if(Input.GetKeyDown(KeyCode.UpArrow)) {rot_z += 90f;}
            if(Input.GetKeyDown(KeyCode.DownArrow)) {rot_z -= 90f;}
        }
    }
}
