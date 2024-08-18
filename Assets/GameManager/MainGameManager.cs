using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;
using Beat;
using Beatboard;

namespace GameManager
{
    public class MainGameManager : MonoBehaviour
    {
        public static BeatboardManager beatboardManager;
        public static BeatManager beatManager;
        private static string _levelName, _levelDescription, _levelAuthor;
        private static int _bpm, _offset;
        public static List<JSONNode> _boards = new();
        private static JSONNode _boardsData;
        private static List<int> _currentBoardPoints = new();
        private static List<float> _currentBoardSizes = new();
        private static List<float> _beatIntervals = new(), _nextBeatTimes = new();
        private float _startTime;
        public static string JsonFilePath;
        public static bool _gameStarted = false;

        public void StartGame()
        {
            if (_gameStarted) return;

            beatboardManager = FindObjectOfType<BeatboardManager>();
            beatManager = FindObjectOfType<BeatManager>();
            JsonFilePath = Path.Combine(Application.streamingAssetsPath, "Levels/1/level.json");

            var jsonFile = File.ReadAllText(JsonFilePath);
            var levelDataJsonNode = JSON.Parse(jsonFile)["Data"];
            _boardsData = JSON.Parse(jsonFile)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"] / 4;
            _offset = (levelDataJsonNode["Offset"]?.AsInt ?? 0) / 1000;

            foreach (var board in levelDataJsonNode["Boards"]) _boards.Add(board);
            CreateBeatboardAtStart(_boards);

            _startTime = Time.time + 0.6f + _offset;
            for (var i = 0; i < _boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / _boards[i]["points"]);
                _nextBeatTimes.Insert(i, _beatIntervals[i]);
            }

            foreach (var t in _boards) _currentBoardPoints.Add(t["points"]);
            foreach (var t in _boards) _currentBoardSizes.Add(t["size"]);
            
            _gameStarted = true;
        }

        private void Start()
        {
            StartGame();
        }

        void Update()
        {
            if (_gameStarted) HandleBeatandBeatboard();
        }

        private static void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            foreach (var board in boards)
            {
                int points = board["points"];
                Vector2 position = new Vector2(board["position"][0], board["position"][1]);
                float size = board["size"];
                beatboardManager.ManageBeatboard(null, -1, points, 0, size, position);
            }
        }

        private void HandleBeatandBeatboard()
        {
            var time = Time.time - _startTime;
            if (_boards == null || _boardsData == null || _beatIntervals == null) return;

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
                if (int.Parse(currentSide) == 1)
                {
                    if (!Mathf.Approximately(currentSize, _currentBoardSizes[i]) && currentSize != 0)
                    {
                        beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], _currentBoardPoints[i], _currentBoardPoints[i],
                            _currentBoardSizes[i], currentSize, new Vector2(_boards[i]["position"][0],
                                _boards[i]["position"][1]));
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
                
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                
                if (int.Parse(currentSide) == currentCycle.Count - 2 && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
                {
                    beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], prevPoint, _currentBoardPoints[i],
                        _currentBoardSizes[i], currentSize, new Vector2(_boards[i]["position"][0],
                            _boards[i]["position"][1]));
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
                beatManager.CreateBeat(i, _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm * 4, size, color);
            }
        }
    }
}