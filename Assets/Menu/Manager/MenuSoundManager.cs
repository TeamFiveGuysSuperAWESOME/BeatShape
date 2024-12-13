using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundManager : MonoBehaviour
{
    AudioSource audioSource;

    public static float musicVolume = 0.2f;
    public static float sfxVolume = 0.1f;

    public AudioClip[] backgroundMusics;
    AudioClip backgroundMusic;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.1f);
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
