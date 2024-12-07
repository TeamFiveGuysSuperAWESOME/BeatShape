using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScreen : MonoBehaviour
{
    public GameObject mask;
    
    public string screenState = "FadeOut";
    float rotation;
    float scale;
    float speed = 0.75f;
    float timer;
    bool playSFX;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        var cameraScale = Mathf.Max(Camera.main.pixelWidth, Camera.main.pixelHeight);
        transform.localScale = new Vector3(cameraScale, cameraScale, 1);
    }

    void FadeIn()
    {
        if(timer == 0) {audioSource.volume = MenuSoundManager.sfxVolume; audioSource.Play();}
        timer += Time.deltaTime;
        rotation = -90*Easing.OutCubic(timer/speed);
        scale = 1-1*Easing.OutCubic(timer/speed);
        
        mask.transform.localScale = new Vector3(scale, scale, 1);
        mask.transform.rotation = Quaternion.Euler(0, 0, rotation);
        if(timer >= speed) {
            screenState = "Idle";
            timer = 0f;
        }
    }

    void FadeOut()
    {
        if(timer == 0) {audioSource.volume = MenuSoundManager.sfxVolume; audioSource.Play();}
        timer += Time.deltaTime;
        rotation = 90*Easing.InCubic(timer/speed);
        scale = 1*Easing.InCubic(timer/speed);
        
        mask.transform.localScale = new Vector3(scale, scale, 1);
        mask.transform.rotation = Quaternion.Euler(0, 0, rotation);
        if(timer >= speed) {
            screenState = "Idle";
            timer = 0f;
        }
    }

    void Update()
    {
        if(screenState == "FadeIn") {FadeIn();}
        else if(screenState == "FadeOut") {FadeOut();}
    }
}
