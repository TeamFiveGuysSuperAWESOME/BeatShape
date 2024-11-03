using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip[] backgroundMusics;
    AudioClip backgroundMusic;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        backgroundMusic = backgroundMusics[Random.Range(0,backgroundMusics.Length)];
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    void Update()
    {
        
    }
}
