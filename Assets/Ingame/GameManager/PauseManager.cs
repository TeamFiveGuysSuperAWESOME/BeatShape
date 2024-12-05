using GameManager;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    public static bool isPaused = false;
    private double pauseTime = 0;

    void Awake()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
               else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        MainGameManager.Paused = true;
        isPaused = true;
        pauseTime = AudioSettings.dspTime;
        GetComponent<AudioSource>().Pause();
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        MainGameManager.Paused = false;
        isPaused = false;
        if (MainGameManager.GameStarted) GameHandler.pauseTime += AudioSettings.dspTime - pauseTime;
        GetComponent<AudioSource>().UnPause();
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
    }
}