using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public float beatTempo = 120f; // Beats per minute
    public bool hasStarted = false;

    private float secPerBeat;
    private float lastBeatTime;

    void Start()
    {
        // Calculate the period for each beat
        secPerBeat = 60f / beatTempo;
    }

    void Update()
    {
        if (!hasStarted) return;

        float currentTime = Time.time;

        // If the time since the last beat is greater than the period for a new beat
        if (currentTime - lastBeatTime >= secPerBeat)
        {
            // Do something on each beat
            DoSomethingOnBeat();

            // Update the last beat time
            lastBeatTime = currentTime;
        }
    }

    void DoSomethingOnBeat()
    {
        // Add your logic here for what should happen on each beat
        Debug.Log("Beat!");
    }

    public void StartBeat()
    {
        hasStarted = true;
        lastBeatTime = Time.time;
    }
}