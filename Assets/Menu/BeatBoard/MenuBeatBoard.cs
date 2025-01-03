using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBeatBoard : MonoBehaviour
{
    MenuManager manager;
    AudioSource audioSource;

    public GameObject beat_prefab;
    public GameObject clack_prefab;
    MenuScenes menuScenes;
    RectTransform menuScenes_rt;
    public GameObject arrows_UD;

    float rot_z;
    float timer;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        menuScenes = GameObject.FindWithTag("scene").GetComponent<MenuScenes>();
        menuScenes_rt = GameObject.FindWithTag("scene").GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        menuScenes_rt.position = new Vector2(0, -150);
        menuScenes.targetPos = menuScenes_rt.position;
        arrows_UD.transform.position = new Vector2(160, -150);
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
        audioSource.volume = MenuSoundManager.sfxVolume;
        audioSource.Play();
    }

    public void Input_up()
    {
        if(manager.sceneState > 0) {
            rot_z -= 90f;
            manager.sceneState -= 1;
            menuScenes.targetPos.y -= 200;
        }
    }
    public void Input_down()
    {
        if(manager.sceneState < 2) {
            rot_z += 90f;
            manager.sceneState += 1;
            menuScenes.targetPos.y += 200;
        }
    }

    void Update()
    {
        if(transform.localScale.x > 150) {
            transform.localScale = new Vector2(transform.localScale.x-20*Time.deltaTime, transform.localScale.y-20*Time.deltaTime);
        }
        else {transform.localScale = new Vector2(150, 150);}

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
                transform.position = new Vector2(-230*inoutCubic, -110+110*inoutCubic);
                rot_z = -180f*inoutCubic;
                transform.rotation = Quaternion.Euler(0, 0, rot_z);

                if(timer > 1.25f) {
                    menuScenes_rt.position = new Vector2(0, -150+150*Easing.InOutCubic((timer-1.25f)/1f));
                    menuScenes.targetPos = menuScenes_rt.position;
                    arrows_UD.transform.position = new Vector2(160, -150+150*Easing.InOutCubic((timer-1.25f)/1f));
                }
            }
            else {
                timer = 0;
                menuScenes.Alpha(1f);
                manager.sceneState = 0;
                manager.menuState = "stageSelect";
            }
        }
        
        if(manager.menuState == "stageSelect") {
            Quaternion targetRotation = Quaternion.Euler(0, 0, rot_z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 6f);
            //input_up / down
        }

        if(manager.menuState == "stageEntry") {
            if(timer < 0.5f) {
                timer += Time.deltaTime;
                transform.position = new Vector3(-230 - 50*Easing.InCubic(timer/0.5f), 0, 0);
                arrows_UD.transform.position = new Vector3(160 + 50*Easing.InCubic(timer/0.5f), 0, 0);
            }

            else {
                timer += Time.deltaTime;
                if(timer > 2f) SceneManager.LoadScene("InGame");
            }
        }
    }
}
