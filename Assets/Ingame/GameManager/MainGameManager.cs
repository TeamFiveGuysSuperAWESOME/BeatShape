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
        private static List<JSONNode> Boards = new();
        private static JSONNode _boardsData;
        private static List<int> _currentBoardPoints = new();
        private static List<float> _beatIntervals = new(), _nextBeatTimes = new(), _currentBoardSizes = new();
        private float _startTime;
        public static string JsonFilePath;
        public static bool GameStarted = false;
        public static bool Paused = false;
        public static bool GameEnded = false;
        public static bool GameReallyEnded = false;
        public static int Score = 0;
        public static int Combo = 0;
        public static float Overload = 0;
        public TextMeshProUGUI scoreText;

        public TextAsset textFile;
        private GameHandler _gameHandler;


        void Awake()
        {
            _beatboardManager = null;
            _beatManager = null;
            _cameraManager = null;
            _levelName = string.Empty;
            _levelDescription = string.Empty;
            _levelAuthor = string.Empty;
            _bpm = 0f;
            _offset = 0f;
            Boards.Clear();
            _boardsData = null;
            _currentBoardPoints.Clear();
            _beatIntervals.Clear();
            _nextBeatTimes.Clear();
            _currentBoardSizes.Clear();
            JsonFilePath = string.Empty;
            GameStarted = false;
            Score = 0;
            Combo = 0;
            Paused = false;
            GameEnded = false;
            GameReallyEnded = false;
        }

        public void StartGame()
        {
            if (GameStarted) return;

            _beatboardManager = FindFirstObjectByType<BeatboardManager>();
            _beatManager = FindFirstObjectByType<BeatManager>();
            _cameraManager = FindFirstObjectByType<CameraManager>();

            textFile = Resources.Load<TextAsset>("Levels/1/level");

            var levelString = textFile.text;
            var levelDataJsonNode = JSON.Parse(levelString)["Data"];
            _boardsData = JSON.Parse(levelString)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"];
            _offset = levelDataJsonNode["Offset"] / 1000f;

            foreach (var board in levelDataJsonNode["Boards"]) Boards.Add(board);
            CreateBeatboardAtStart(Boards);

            _startTime = 0.6f + _offset;
            for (var i = 0; i < Boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / Boards[i]["points"]);
                _nextBeatTimes.Insert(i, _beatIntervals[i]);
            }

            foreach (var t in Boards) _currentBoardPoints.Add(t["points"]);
            foreach (var t in Boards) _currentBoardSizes.Add(t["size"]);

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

        private void Start()
        {
            StartGame();
            StartCoroutine(DecreaseOverloadRoutine());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SceneManager.LoadScene("MainMenu");
            }
            if (!GameStarted) {
                GameObject.FindWithTag("countdown").GetComponent<TextMeshProUGUI>().text = "Space to Start";
                GameObject.FindWithTag("countdown").GetComponent<CountDownManager>().RefreshTimer(60f/_bpm, 0.6f+_offset, Boards[0]["points"]);
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
                GameEnded = true;
                GameReallyEnded = true;
            }
            if (GameReallyEnded)
            {
                SceneManager.LoadScene("MainMenu");
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
            if (Overload >= 0.3f && Overload < 3)
            {
                Overload -= 0.3f;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}