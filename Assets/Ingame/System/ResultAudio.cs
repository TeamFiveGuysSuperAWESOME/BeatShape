using UnityEngine;

public class ResultAudio : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        audioSource.volume = MenuSoundManager.sfxVolume;
    }

    public void Audio_Snare()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }
    public void Audio_Cymbal()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
    }
}
