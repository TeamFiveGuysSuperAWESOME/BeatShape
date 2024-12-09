using System.Collections;
using GameManager;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject practiceRestartButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject practiceBar;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI offsetSettings;
    [SerializeField] private TextMeshProUGUI countdown;
    public static bool isPaused = false;
    public static int sceneIndex = 0;
    private double pauseTime = 0;

    void Awake()
    {
        sceneIndex = 0;
        isPaused = false;
        pausePanel.SetActive(false);
        offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
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

    public void ToggleSettings() {
        sceneIndex = sceneIndex == 0 ? 1 : 0;
        StopAllCoroutines();
        StartCoroutine(SettingsAnimation());
    }

    public void OffsetPlusTen() {
        PlayerPrefs.SetFloat("calibratedOffset", PlayerPrefs.GetFloat("calibratedOffset") + 0.01f);
        offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
        GetComponent<AudioSource>().time -= 0.01f;
    }

    public void OffsetMinusTen() {
        PlayerPrefs.SetFloat("calibratedOffset", PlayerPrefs.GetFloat("calibratedOffset") - 0.01f);
        offsetSettings.text = Mathf.Round(PlayerPrefs.GetFloat("calibratedOffset") * 1000).ToString();
        GetComponent<AudioSource>().time += 0.01f;
    }

    private IEnumerator SettingsAnimation() {
        if (sceneIndex == 1) {
            float duration = 0.5f;
            float t = 0;
            Vector3 startPos1 = resumeButton.transform.position;
            Vector3 startPos2 = settingsButton.transform.position;
            Vector3 startPos3 = quitButton.transform.position;
            Vector3 startPos4 = settingsPanel.transform.position;
            Vector3 endPos1 = new Vector3(-120, 60, resumeButton.transform.position.z);
            Vector3 endPos2 = new Vector3(-120, 0, settingsButton.transform.position.z);
            Vector3 endPos3 = new Vector3(-120, -60, quitButton.transform.position.z);
            Vector3 endPos4 = new Vector3(50, 150, settingsPanel.transform.position.z);
            Vector3 startScale1 = restartButton.transform.localScale;
            Vector3 startScale2 = practiceRestartButton.transform.localScale;
            Vector3 startScale3 = practiceBar.transform.localScale;
            while (t < duration) {
                t += Time.deltaTime;
                resumeButton.transform.position = Vector3.Lerp(startPos1, endPos1, Easing.Ease(t / duration, "outcubic"));
                settingsButton.transform.position = Vector3.Lerp(startPos2, endPos2, Easing.Ease(t / duration, "outcubic"));
                quitButton.transform.position = Vector3.Lerp(startPos3, endPos3, Easing.Ease(t / duration, "outcubic"));
                settingsPanel.transform.position = Vector3.Lerp(startPos4, endPos4, Easing.Ease(t / duration, "outcubic"));
                restartButton.transform.localScale = Vector3.Lerp(startScale1, Vector3.zero, Easing.Ease(t / duration, "outcubic"));
                practiceRestartButton.transform.localScale = Vector3.Lerp(startScale2, Vector3.zero, Easing.Ease(t / duration, "outcubic"));
                practiceBar.transform.localScale = Vector3.Lerp(startScale3, Vector3.zero, Easing.Ease(t / duration, "outcubic"));
                yield return null;
            }
        }
        else if (sceneIndex == 0) 
        {
            float duration = 0.5f;
            float t = 0;
            Vector3 startPos1 = resumeButton.transform.position;
            Vector3 startPos2 = settingsButton.transform.position;
            Vector3 startPos3 = quitButton.transform.position;
            Vector3 startPos4 = settingsPanel.transform.position;
            Vector3 endPos1 = new Vector3(-120, 10, resumeButton.transform.position.z);
            Vector3 endPos2 = new Vector3(60, 10, settingsButton.transform.position.z);
            Vector3 endPos3 = new Vector3(120, 10, quitButton.transform.position.z);
            Vector3 endPos4 = new Vector3(50, 0, settingsPanel.transform.position.z);
            Vector3 startScale1 = restartButton.transform.localScale;
            Vector3 startScale2 = practiceRestartButton.transform.localScale;
            Vector3 startScale3 = practiceBar.transform.localScale;
            while (t < duration) {
                t += Time.deltaTime;
                resumeButton.transform.position = Vector3.Lerp(startPos1, endPos1, Easing.Ease(t / duration, "outcubic"));
                settingsButton.transform.position = Vector3.Lerp(startPos2, endPos2, Easing.Ease(t / duration, "outcubic"));
                quitButton.transform.position = Vector3.Lerp(startPos3, endPos3, Easing.Ease(t / duration, "outcubic"));
                settingsPanel.transform.position = Vector3.Lerp(startPos4, endPos4, Easing.Ease(t / duration, "outcubic"));
                restartButton.transform.localScale = Vector3.Lerp(startScale1, Vector3.one, Easing.Ease(t / duration, "outcubic"));
                practiceRestartButton.transform.localScale = Vector3.Lerp(startScale2, Vector3.one, Easing.Ease(t / duration, "outcubic"));
                practiceBar.transform.localScale = Vector3.Lerp(startScale3, new Vector3(20, 20, 1), Easing.Ease(t / duration, "outcubic"));
                yield return null;
            }
        }
    }
}