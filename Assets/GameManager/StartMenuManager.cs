using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManager
{
    public class StartMenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Ingame");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            MainGameManager mainGameManager = FindObjectOfType<MainGameManager>();
            if (mainGameManager != null)
            {
                mainGameManager.StartGame();
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
        }
    }
}
