using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatData : MonoBehaviour
{
    public float angle_offset;
    public float bpm = 120f;
    float timer = 0f;

    void Awake()
    {
        
    }

    void Refresh()
    {
        GameObject targetBoard = transform.parent.gameObject;
        Vector2 targetPos = targetBoard.transform.position;
        beatboardData targetData = targetBoard.GetComponent<beatboardData>();
        float targetSize = targetData.size;
        Debug.Log(timer);
        Debug.Log(timer >= bpm / 60); // BPM = 120
        
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        Refresh();
        if(timer >= bpm / 60)
        {
            //Destroy(gameObject);
        }
    }
}
