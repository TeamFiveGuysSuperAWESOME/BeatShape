using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public GameObject beatPrefab;
    void Start()
    {
        Instantiate(beatPrefab);
    }

    void Update()
    {
        
    }
}