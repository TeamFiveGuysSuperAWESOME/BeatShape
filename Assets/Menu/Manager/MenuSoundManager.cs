using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundManager : MonoBehaviour
{
    AudioSource audioSource;

    public float musicVolume = 0.2f;
    public float sfxVolume = 0.2f;

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
        audioSource.volume = musicVolume;
    }
}
