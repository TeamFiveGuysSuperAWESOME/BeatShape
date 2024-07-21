using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatboardManager : MonoBehaviour
{
    LineRenderer lr;
    public GameObject beatboardPrefab;
    public List<GameObject> beatboards;

    public void createBeatboard(float points, float size, Vector2 position)
    {
        removeBeatboard();
        GameObject beatboardObject = Instantiate(beatboardPrefab, position, Quaternion.identity, transform);
        beatboardData bbdata = beatboardObject.GetComponent<beatboardData>();
        bbdata.points = points;
        bbdata.size = size;
        beatboardObject.name = "Beatboard " + points;
        beatboards.Add(beatboardObject);

        lr = beatboardObject.GetComponent<LineRenderer>();
        lr.positionCount = (int)points + 1; // Add 1 to close the shape
        float angleStep = 360f / points;

        for (int i = 0; i <= points; i++) // Iterate up to points + 1 to close the shape
        {
            float angle = (90f + i * angleStep) * Mathf.Deg2Rad;
            lr.SetPosition(i, new Vector2(bbdata.size * Mathf.Cos(angle), bbdata.size * Mathf.Sin(angle)));
        }
    }

    public IEnumerator updateBeatboard(int currentPoints, int nextPoints, float size, Vector2 position)
    {
        int diff = Math.Abs(currentPoints - nextPoints);
        if (currentPoints > nextPoints)
        {
            for (float i = 0; i <= diff*100; i += diff)
            {
                createBeatboard(currentPoints-(i/100), size, position);
                yield return new WaitForSeconds(0f);
            }
        } else if (currentPoints < nextPoints)
        {
            for (float i = 0; i <= diff*100; i += diff)
            {
                createBeatboard(currentPoints+(i/100), size, position);
                yield return new WaitForSeconds(0f);
            }
        }
        createBeatboard(nextPoints, size, position);
    }

    public void removeBeatboard()
    {
        while (beatboards.Count > 0)
        {
            GameObject beatboardToRemove = beatboards[0];
            beatboards.RemoveAt(0);
            Destroy(beatboardToRemove);
        }
    }

    void Start()
    {
        createBeatboard(4.012f, 20f, new Vector2(0, 0));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            StartCoroutine(updateBeatboard(4, 3, 20f, new Vector2(0, 0)));
        } else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(updateBeatboard(3, 4, 20f, new Vector2(0, 0)));
        }
    }
}
