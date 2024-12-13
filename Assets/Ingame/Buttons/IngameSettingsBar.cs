using UnityEngine;
using TMPro;
using GameManager;
using Beat;

public class IngameSettingsBar : MonoBehaviour
{
    bool onClick = false;
    public string type = "Music";
    float timer;
    
    public GameObject bar_front;
    LineRenderer lr;
    AudioSource audioSource;
    public GameObject text;
    TextMeshProUGUI text_tmp;

    void Awake()
    {
        lr = bar_front.GetComponent<LineRenderer>();
        text_tmp = text.GetComponent<TextMeshProUGUI>();
        if(type == "SoundEffect") {audioSource = GetComponent<AudioSource>();}
    }
    void Start()
    {
        lr.SetPosition(1, new Vector3(type == "Music" ? MenuSoundManager.musicVolume*10 : MenuSoundManager.sfxVolume*10,0,0));
        text_tmp.text = (lr.GetPosition(1).x*10).ToString("F0") + "%";
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
        if(PauseManager.sceneIndex == 1) {
            if(onClick) {
                Vector3 newPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,0,0);
                newPos = new Vector3(transform.InverseTransformPoint(newPos).x + 0.5f,0,0);
                if(newPos.x < 0) {lr.SetPosition(1, new Vector3(0,0,0));}
                else if(newPos.x > 1) {lr.SetPosition(1, new Vector3(10,0,0));}
                else {lr.SetPosition(1, newPos*10);}
                text_tmp.text = (lr.GetPosition(1).x*10).ToString("F0") + "%";
                
                if(type == "Music") 
                {
                    MenuSoundManager.musicVolume = lr.GetPosition(1).x/10;
                    MainGameManager.ChangeMusicVolume();
                    PlayerPrefs.SetFloat("MusicVolume", MenuSoundManager.musicVolume);
                }
                else if(type == "SoundEffect") {
                    timer = timer<0 ? timer+Time.deltaTime : 0;
                    if(MenuSoundManager.sfxVolume != lr.GetPosition(1).x/10 && timer == 0) {
                        timer = -0.05f;
                        audioSource.volume = lr.GetPosition(1).x/10;
                        audioSource.Play();
                    }
                    MenuSoundManager.sfxVolume = lr.GetPosition(1).x/10;
                    BeatManager.ChangeSfxVolume();
                    PlayerPrefs.SetFloat("SfxVolume", MenuSoundManager.sfxVolume);
                }
            }
        }
    }
}
