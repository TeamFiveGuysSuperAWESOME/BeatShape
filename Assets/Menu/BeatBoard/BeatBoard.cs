using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBoard : MonoBehaviour
{
    float rot_z;

    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, rot_z);
        rot_z += 10*Time.deltaTime;
    }
}
