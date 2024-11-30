using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;
using System.Net.Mime;
using Beat;
using Beatboard;
using StreamingAssets.Levels;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.InteropServices;
using System.Data;
using Unity.VisualScripting;

namespace GameManager
{
    public class MainGameManager : MonoBehaviour
    {
        private static BeatboardManager _beatboardManager;
        private static BeatManager _beatManager;
        private static CameraManager _cameraManager;
        private static string _levelName, _levelDescription, _levelAuthor;
        private static float _bpm;
        private static float _offset;
        public static Color BeatboardColor;
        private static List<JSONNode> Boards = new();
        private static JSONNode _boardsData;
        private static List<int> _currentBoardPoints = new();
        private static List<float> _beatIntervals = new(), _nextBeatTimes = new(), _currentBoardSizes = new();
        private float _startTime;
        public static string JsonFilePath;
        private bool _isJsonFileLoaded = false;
        public static bool GameStarted = false;
        public static bool Paused = false;
        public static bool GameEnded = false;
        public static bool GameReallyEnded = false;
        public static int Score = 0;
        //public static int Combo = 0;
        public static float Overload = 0f;
        public TextMeshProUGUI scoreText;
        private FadeInScreen screen;
        public TextAsset textFile;
        private GameHandler _gameHandler;
        public static bool DebugMode = MenuManager.DebugMode;
        private bool _isLeaving = false;
        private float animtimer = 0f;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void UploadFile();
#endif

        private string levelJsonData;

        void Awake()
        {
            screen = GameObject.FindWithTag("screen").GetComponent<FadeInScreen>();
            _beatboardManager = null;
            _beatManager = null;
            _cameraManager = null;
            _levelName = string.Empty;
            _levelDescription = string.Empty;
            _levelAuthor = string.Empty;
            _bpm = 0f;
            _offset = 0f;
            BeatboardColor = Color.white;
            Boards.Clear();
            _boardsData = null;
            _currentBoardPoints.Clear();
            _beatIntervals.Clear();
            _nextBeatTimes.Clear();
            _currentBoardSizes.Clear();
            JsonFilePath = string.Empty;
            GameStarted = false;
            Score = 0;
            //Combo = 0;
            Paused = false;
            GameEnded = false;
            GameReallyEnded = false;
            DebugMode = MenuManager.DebugMode;
        }

        void Start()
        {
            Debug.Log("Debug Mode: " + DebugMode);
            if (DebugMode)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                UploadFile();
#endif
            }
#if UNITY_EDITOR
            StartGame();
#endif

            StartCoroutine(DecreaseOverloadRoutine());
        }

        public void OnFileUpload(string jsonData)
        {
            levelJsonData = jsonData;
            StartGame();
            _isJsonFileLoaded = true;
        }

        public void StartGame()
        {
            if (GameStarted) return;

            _beatboardManager = FindFirstObjectByType<BeatboardManager>();
            _beatManager = FindFirstObjectByType<BeatManager>();
            _cameraManager = FindFirstObjectByType<CameraManager>();

            textFile = Resources.Load<TextAsset>("Levels/1/level");
            //textFile = 
            if (DebugMode)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                var levelString = levelJsonData;
#endif
            }

#if UNITY_EDITOR
            var levelString = textFile.text;
#endif
            var levelDataJsonNode = JSON.Parse(levelString)["Data"];
            _boardsData = JSON.Parse(levelString)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            var levelAudio = levelDataJsonNode["AudioFile"];
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Levels/1/" + levelAudio);
            _bpm = levelDataJsonNode["Bpm"];
            _offset = levelDataJsonNode["Offset"] / 1000f;

            foreach (var board in levelDataJsonNode["Boards"]) Boards.Add(board);
            CreateBeatboardAtStart(Boards);

            _startTime = 0.5f + _offset;
            for (var i = 0; i < Boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / Boards[i]["points"]);
                _nextBeatTimes.Insert(i, _beatIntervals[i]);
            }

            foreach (var t in Boards) _currentBoardPoints.Add(t["points"]);
            foreach (var t in Boards) _currentBoardSizes.Add(t["size"]);

            var background = levelDataJsonNode["Background"];
            BeatboardColor = new Color(background["BBColor"][0], background["BBColor"][1], background["BBColor"][2]);
            Camera.main.backgroundColor = new Color(background["BGColor"][0], background["BGColor"][1], background["BGColor"][2]);
            if (background["BGImage"] != null)
            {
                var backgroundSprite = Resources.Load<Sprite>("Levels/1/" + background["BGImage"]);
                if (backgroundSprite != null)
                {
                    GameObject backgroundObj = new("Background");
                    backgroundObj.transform.SetParent(Camera.main.transform, false);
                    backgroundObj.tag = "BackgroundImg";
                    SpriteRenderer spriteRenderer = backgroundObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = backgroundSprite;
                    float screenHeight = Camera.main.orthographicSize * 2;
                    float screenWidth = screenHeight * Screen.width / Screen.height;
                    Vector2 spriteSize = backgroundSprite.bounds.size;
                    float scale = Mathf.Max(screenWidth / spriteSize.x, screenHeight / spriteSize.y);
                    backgroundObj.transform.localScale = new Vector3(scale, scale, 1);
                    backgroundObj.transform.position = new Vector3(0, 0, 2);
                    spriteRenderer.sortingOrder = -1;
                }
            }

            _gameHandler = gameObject.AddComponent<GameHandler>();
            _gameHandler.Initialize(
                _beatboardManager,
                _beatManager,
                _cameraManager,
                Boards,
                _boardsData,
                _currentBoardPoints,
                _beatIntervals,
                _nextBeatTimes,
                _currentBoardSizes,
                _startTime,
                _bpm
            );
            
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                LoadMainMenu();
            }
            if (_isLeaving) 
            {
                screen.screenState = "FadeIn";
                animtimer += Time.deltaTime;
                if (animtimer > 0.75f) { _isLeaving = false; SceneManager.LoadScene("MainMenu"); }
            }

            if (!GameStarted) {
                GameObject.FindWithTag("countdown").GetComponent<TextMeshProUGUI>().text = "Space to Start";
                if (_isJsonFileLoaded) GameObject.FindWithTag("countdown").GetComponent<CountDownManager>().RefreshTimer(60f/_bpm, 0.6f+_offset, Boards[0]["points"]);
                if(Input.GetKeyDown(KeyCode.Space)) {
                    GameStarted = true;
                    GetComponent<AudioSource>().Play();
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Paused = !Paused;
                if (Paused)
                {
                    Time.timeScale = 0;
                    GetComponent<AudioSource>().Pause();
                }
                else
                {
                    Time.timeScale = 1;
                    GetComponent<AudioSource>().Play();
                }
            }

            _gameHandler.HandleGame();
            scoreText.text = Overload.ToString();
            if (Overload >= 3)
            {
                Debug.Log("Game Over");
            }
        }

        private static void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            foreach (var board in boards)
            {
                int points = board["points"];
                Vector2 position = new Vector2(board["position"][0], board["position"][1]);
                float size = board["size"];
                _beatboardManager.ManageBeatboard(null, -1, points, 0, size, position);
            }
        }

        private IEnumerator DecreaseOverloadRoutine()
        {
            while (!GameReallyEnded) 
            {
                if (Overload >= 0.5f)
                {
                    Overload -= 0.5f;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void LoadMainMenu()
        {
            animtimer = 0f;
            _isLeaving = true;
        }
    }
}