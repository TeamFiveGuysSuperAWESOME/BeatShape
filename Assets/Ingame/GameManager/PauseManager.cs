using System.Collections;
using GameManager;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private TextMeshProUGUI countdown;
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
        Camera.main.GetComponent<PostProcessLayer>().enabled = false;
        pauseTime = AudioSettings.dspTime;
        GetComponent<AudioSource>().Pause();
        pausePanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        StartCoroutine(ResumeGameCoroutine());
    }

    private IEnumerator ResumeGameCoroutine() {
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        Camera.main.GetComponent<PostProcessLayer>().enabled = true;
        if (MainGameManager.GameStarted) {
            for (int i = 3; i > 0; i--) {
                countdown.text = i.ToString();
                yield return new WaitForSeconds(0.75f);
            }
            countdown.text = "";
        }
        MainGameManager.Paused = false;
        isPaused = false;
        if (MainGameManager.GameStarted) {
            GameHandler.pauseTime += AudioSettings.dspTime - pauseTime;
            GetComponent<AudioSource>().UnPause();
        }
    }
}