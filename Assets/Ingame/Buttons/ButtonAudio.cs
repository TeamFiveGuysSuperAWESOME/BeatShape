using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        audioSource.volume = MenuSoundManager.sfxVolume;
    }

    public void Audio_Button()
    {
        audioSource.Play();
    }
}
