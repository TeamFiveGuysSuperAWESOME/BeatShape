using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BeatManager : MonoBehaviour
{
    public GameObject beatPrefab;
    private Random random = new Random();

    public void createBeat(int sides, int side, float speed)
    {
        GameObject beatObject = Instantiate(beatPrefab, new Vector2(0, 0), Quaternion.identity, transform);
        int angle = 360 / sides * side;
        
        //set beatData
        BeatData beatData = beatObject.GetComponent<BeatData>();
        beatData.angle = angle;
        beatData.speed = speed;
        
        float angleRadians = angle * Mathf.Deg2Rad;

        // Calculate direction vector
        Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        beatObject.transform.Translate(direction * 2);

        // Move the beatObject in the Update method
        beatObject.GetComponent<BeatMovement>().SetMovement(direction, speed);
    }
    private void Start()
    {
        // Start invoking the CreateBeatWrapper method repeatedly after a delay of beatInterval
        InvokeRepeating("CreateBeatWrapper", 1f, 1f);
    }

    private void CreateBeatWrapper()
    {
        int side = random.Next(4);
        createBeat(4, side, 10f);
    }
    void Update()
    {
        
    }
}