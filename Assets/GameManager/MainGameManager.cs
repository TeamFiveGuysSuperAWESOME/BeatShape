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

        public void StartGame()
        {
            if (GameStarted) return;

            _beatboardManager = FindObjectOfType<BeatboardManager>();
            _beatManager = FindObjectOfType<BeatManager>();
            //JsonFilePath = Path.Combine(Application.streamingAssetsPath, "Levels/1/level.json");

            //var jsonFile = File.ReadAllText(JsonFilePath);
            var levelString = LevelManager.Level1;
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
        }

        void Update()
        {
            if (!GameStarted) return; 
            HandleBeatandBeatboard();
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

        private void HandleBeatandBeatboard()
        {
            var time = Time.time - _startTime;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (time < _nextBeatTimes[i]) continue;

                var currentCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((time / _beatIntervals[i] - 1) / _currentBoardPoints[i]) + 1)];
                var prevCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((time / _beatIntervals[i] - 1) / _currentBoardPoints[i]))];
                var currentSide = (Mathf.FloorToInt((time / _beatIntervals[i] - 1) % _currentBoardPoints[i]) + 1).ToString();
                if (currentCycle == null || currentCycle[currentSide] == null) continue;

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                if (int.Parse(currentSide) == currentCycle.Count - 2 && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
                {
                    _beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], prevPoint, _currentBoardPoints[i],
                        _currentBoardSizes[i], currentSize, new Vector2(Boards[i]["position"][0],
                            Boards[i]["position"][1]));
                }
                if (int.Parse(currentSide) == 1)
                {
                    if (!Mathf.Approximately(currentSize, _currentBoardSizes[i]) && currentSize != 0)
                    {
                        _beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], _currentBoardPoints[i], _currentBoardPoints[i],
                            _currentBoardSizes[i], currentSize, new Vector2(Boards[i]["position"][0],
                                Boards[i]["position"][1]));
                        _currentBoardSizes[i] = currentSize;
                    }
                    if (currentPoint != _currentBoardPoints[i] && currentPoint != 0)
                    {
                        _currentBoardPoints[i] = currentPoint;
                        currentCycle = _boardsData[i][Mathf.FloorToInt((time / _beatIntervals[i] - 1) / currentPoint)];
                        currentSide = (Mathf.FloorToInt((time / _beatIntervals[i] - 1) % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                    }
                }

                _nextBeatTimes[i] += _beatIntervals[i];

                if (!currentCycle[currentSide]["Beat"]) continue;

                float size = (currentCycle[currentSide]["Size"] != null ? currentCycle[currentSide]["Size"].AsFloat : 1) * BeatboardManager.GetBeatboardSize(i) / 20f;
                Color color = new Color(
                    currentCycle[currentSide]["Color"]?[0]?.AsFloat ?? 1,
                    currentCycle[currentSide]["Color"]?[1]?.AsFloat ?? 1,
                    currentCycle[currentSide]["Color"]?[2]?.AsFloat ?? 1
                );
                if (color == Color.black) color = Color.white;
                float speed = currentCycle[currentSide]["Speed"] != null ? currentCycle[currentSide]["Speed"].AsFloat : 1;
                _beatManager.CreateBeat(i, _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm * 4, size, color);
            }
        }
    }
}