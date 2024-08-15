using System.Collections.Generic;
using Beat;
using Beatboard;
using SimpleJSON;
using UnityEngine;
using System.IO;

namespace GameManager
{
    public class MainGameManager : MonoBehaviour
    {
        public static BeatboardManager beatboardManager;
        public static BeatManager beatManager;
        private static string _levelName;
        private static string _levelDescription;
        private static string _levelAuthor;
        private static int _bpm;
        public static List<JSONNode> _boards = new();
        private static List<JSONNode> _boardsData = new();
        private static List<float> _beatIntervals = new();
        private static List<float> _nextBeatTimes = new();
        public static string jsonFilePath;

        public static void StartGame()
        {
            beatboardManager = FindObjectOfType<BeatboardManager>();
            beatManager = FindObjectOfType<BeatManager>();
            
            jsonFilePath = Path.Combine(Application.streamingAssetsPath, "Levels/1/level.json");

            // Load the JSON file
            var jsonFile = File.ReadAllText(jsonFilePath);
            var levelDataJsonNode = JSON.Parse(jsonFile)["Data"];
            var boardsDataJsonNode = JSON.Parse(jsonFile)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"];
            
            foreach (var board in levelDataJsonNode["Boards"])
            {
                _boards.Add(board);
            }
            CreateBeatboardAtStart(_boards);

            foreach (var boardData in boardsDataJsonNode)
            {
                _boardsData.Add(boardData);
            }
            for (var i = 0; i < _boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / _boards[i]["points"]);
                _nextBeatTimes.Insert(i, Time.time + _beatIntervals[i]);
            }
        }

        void Start()
        {
            StartGame();
            
        }

        void Update()
        {
            Debug.Log(_beatIntervals[0] + " " + _beatIntervals[1] + " " + _nextBeatTimes[0] + " " + _nextBeatTimes[1]);
            HandleBeatCreation();
        }

        public static void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            foreach (var board in boards)
            {
                int points = board["points"];
                Vector2 position = new Vector2(board["position"][0], board["position"][1]);
                float size = board["size"];
                beatboardManager.ManageBeatboard(null, -1, points, size, position);
            }
        }

        private void HandleBeatCreation()
{
    var time = Time.time;
    if (_boards == null || _boardsData == null || _beatIntervals == null) return;

    for (var i = 0; i < _boardsData.Count; i++)
    {
        if (!(time >= _nextBeatTimes[i])) continue;
        _nextBeatTimes[i] += _beatIntervals[i];

        var points = (int)BeatboardManager.GetBeatboardPoints(i);
        if (points == 0) continue;
        var currentCircle = _boardsData[i][Mathf.FloorToInt((time / _beatIntervals[i] - 1) / points)];
        var nextCircle = _boardsData[i][Mathf.FloorToInt((time / _beatIntervals[i] - 1) / points) + 1];
        var currentSide = Mathf.FloorToInt((time / _beatIntervals[i] - 1) % points) + 1;
        if (currentCircle == null || currentCircle[currentSide] == null) continue;
        
        int nextPoints = currentCircle["Points"]?.AsInt ?? points;
        if (currentSide == 1 && nextPoints != points)
        {
            beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], points, nextPoints,
                _boards[i]["size"],
                new Vector2(_boards[i]["position"][0], _boards[i]["position"][1]));
            points = (int)BeatboardManager.GetBeatboardPoints(i);
            currentCircle = _boardsData[i][Mathf.FloorToInt((time / _beatIntervals[i] - 1) / points)];
            currentSide = Mathf.FloorToInt((time / _beatIntervals[i] - 1) % points) + 1;
            _beatIntervals[i] = 60f / _bpm / nextPoints;
        }
        
        if (currentCircle[currentSide]["Beat"])
        {
            float size = currentCircle[currentSide]["Size"] != null ? currentCircle[currentSide]["Size"].AsFloat : 1;
            Color color = new Color(
                currentCircle[currentSide]["Color"]?[0]?.AsFloat ?? 1,
                currentCircle[currentSide]["Color"]?[1]?.AsFloat ?? 1,
                currentCircle[currentSide]["Color"]?[2]?.AsFloat ?? 1
            );
            if (color.r == 0 && color.g == 0 && color.b == 0) color = Color.white;
            float speed = currentCircle[currentSide]["Speed"] != null ? currentCircle[currentSide]["Speed"].AsFloat : 1;
            beatManager.CreateBeat(i, currentSide, speed, 60f / _beatIntervals[i], size, color);
        }
    }
}
    }
}