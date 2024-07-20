using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatboardManager : MonoBehaviour
{
    LineRenderer lr;
    public GameObject beatboardPrefab;
    int bbcount = 0;

    public void createBeatboard(int points, float size, Vector2 position)
    {
        GameObject beatboardObject = Instantiate(beatboardPrefab, position, Quaternion.identity, transform);
        beatboardData bbdata = beatboardObject.GetComponent<beatboardData>();
        bbdata.points = points;
        bbdata.size = size;
        beatboardObject.name = "Beatboard " + bbcount;
        bbcount++;

        lr = beatboardObject.GetComponent<LineRenderer>();
        lr.positionCount = bbdata.points;
        for(int i = 0; i < bbdata.points; i++)
        {
            lr.SetPosition(i, new Vector2(bbdata.size * Mathf.Cos((90f + i * 360 / bbdata.points) * Mathf.Deg2Rad), bbdata.size * Mathf.Sin((90f + i * 360 / bbdata.points) * Mathf.Deg2Rad)));
        }
    }

    void Start()
    {
        createBeatboard(4, 10f, new Vector2(0, 30));
        createBeatboard(5, 10f, new Vector2(0, 0));
        createBeatboard(6, 10f, new Vector2(0, -30));
        createBeatboard(20, 10f, new Vector2(0, -60));
    }

    void Update()
    {
        
    }
}
