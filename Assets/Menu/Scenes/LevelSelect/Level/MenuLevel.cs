using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevel : MonoBehaviour
{
    public Vector3 targetScale;

    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 6f);
    }
}
