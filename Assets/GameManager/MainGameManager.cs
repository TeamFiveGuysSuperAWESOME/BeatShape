using System.Collections.Generic;
using Beat;
using Beatboard;
using Levels;
using SimpleJSON;
using UnityEngine;

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
        private static List<JSONNode> _boards = new List<JSONNode>();
        private static List<JSONNode> _boardsData = new List<JSONNode>();
        private static float _beatInterval;
        private static float _nextBeatTime;

        public static void StartGame()
        {
            Debug.Log("StartGame method called");
            beatboardManager = FindObjectOfType<BeatboardManager>();
            beatManager = FindObjectOfType<BeatManager>();
            var jsonFile = LevelManager.level1;
            var levelDataJsonNode = JSON.Parse(jsonFile)["Data"];
            var boardsDataJsonNode = JSON.Parse(jsonFile)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"];
            _beatInterval = 60f / _bpm;
            _nextBeatTime = Time.time + _beatInterval;

            foreach (var board in levelDataJsonNode["Boards"])
            {
                _boards.Add(board);
            }
            CreateBeatboardAtStart(_boards);

            foreach (var boardData in boardsDataJsonNode)
            {
                _boardsData.Add(boardData);
                Debug.Log(boardData);
            }
        }

        void Start()
        {
            StartGame();
        }

        void Update()
        {
            HandleBeatCreation();
        }

        public static void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            Debug.Log("Creating beatboards");
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
            if (!(Time.time >= _nextBeatTime)) return;
            _nextBeatTime += _beatInterval;
            var time = Time.time;
            for (var i = 0; i < _boardsData.Count; i++)
            {
                var currentCircle = _boardsData[i][Mathf.FloorToInt((time / _beatInterval - 1) / _boards[i]["points"])];
                var currentSide = Mathf.FloorToInt((time / _beatInterval - 1) % _boards[i]["points"]);
                if (currentCircle[currentSide][0])
                {
                    beatManager.CreateBeat(i, currentSide, 10f);
                }
            }
        }
    }
}