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

    void FadeIn()
    {
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

        if(Input.GetKeyDown(KeyCode.O)) {screenState = "FadeIn";}
        if(Input.GetKeyDown(KeyCode.P)) {screenState = "FadeOut";}
    }
}
