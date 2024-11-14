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

namespace GameManager
{
    public class MainGameManager : MonoBehaviour
    {
        private static BeatboardManager _beatboardManager;
        private static BeatManager _beatManager;
        private static string _levelName, _levelDescription, _levelAuthor;
        private static int _bpm, _offset;
        private static List<JSONNode> Boards = new();
        private static JSONNode _boardsData;
        private static List<int> _currentBoardPoints = new();
        private static List<float> _beatIntervals = new(), _nextBeatTimes = new(), _currentBoardSizes = new();
        private float _startTime;
        public static string JsonFilePath;
        public static bool GameStarted = false;
        public static int Score = 0;
        public static int Combo = 0;
        public TextMeshProUGUI scoreText;

        public TextAsset textFile;
        private BeatHandler _beatHandler;

        public void StartGame()
        {
            if (GameStarted) return;

            _beatboardManager = FindFirstObjectByType<BeatboardManager>();
            _beatManager = FindFirstObjectByType<BeatManager>();
            //JsonFilePath = Path.Combine(Application.streamingAssetsPath, "Levels/1/level.json");

            //var jsonFile = File.ReadAllText(JsonFilePath);

            textFile = Resources.Load<TextAsset>("Levels/1/level");

            var levelString = textFile.text;
            var levelDataJsonNode = JSON.Parse(levelString)["Data"];
            _boardsData = JSON.Parse(levelString)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"] / 4;
            _offset = (levelDataJsonNode["Offset"]?.AsInt ?? 0) / 1000;

            foreach (var board in levelDataJsonNode["Boards"]) Boards.Add(board);
            CreateBeatboardAtStart(Boards);

            _startTime = Time.time + 0.6f + _offset;
            for (var i = 0; i < Boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / Boards[i]["points"]);
                _nextBeatTimes.Insert(i, _beatIntervals[i]);
            }

            foreach (var t in Boards) _currentBoardPoints.Add(t["points"]);
            foreach (var t in Boards) _currentBoardSizes.Add(t["size"]);
            
            GameStarted = true;
        }

        private void Start()
        {
            StartGame();

            _beatHandler = gameObject.AddComponent<BeatHandler>();
            _beatHandler.Initialize(
                _beatboardManager,
                _beatManager,
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
            if (!GameStarted) return; 
            _beatHandler.HandleGame();
            scoreText.text = Score.ToString();
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
    }
}