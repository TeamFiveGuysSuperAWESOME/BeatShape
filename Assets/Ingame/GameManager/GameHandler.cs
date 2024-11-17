using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Beat;
using Beatboard;

namespace GameManager {
    public class GameHandler : MonoBehaviour
    {
        private BeatboardManager _beatboardManager;
        private BeatManager _beatManager;
        private CameraManager _cameraManager;
        private List<JSONNode> Boards;
        private JSONNode _boardsData;
        private List<int> _currentBoardPoints;
        private List<float> _beatIntervals;
        private List<float> _nextBeatTimes;
        private List<float> _currentBoardSizes;
        private float _startTime;
        private int _bpm;
        private float timer;
        private float _elapsedTime = 0f;

        public void Initialize(
            BeatboardManager beatboardManager,
            BeatManager beatManager,
            CameraManager cameraManager,
            List<JSONNode> boards,
            JSONNode boardsData,
            List<int> currentBoardPoints,
            List<float> beatIntervals,
            List<float> nextBeatTimes,
            List<float> currentBoardSizes,
            float startTime,
            int bpm
        )
        {
            _beatboardManager = beatboardManager;
            _beatManager = beatManager;
            _cameraManager = cameraManager;
            Boards = boards;
            _boardsData = boardsData;
            _currentBoardPoints = currentBoardPoints;
            _beatIntervals = beatIntervals;
            _nextBeatTimes = nextBeatTimes;
            _currentBoardSizes = currentBoardSizes;
            _startTime = startTime;
            _bpm = bpm;
        }

        public void HandleGame()
        {
            _elapsedTime += Time.deltaTime;
            var timeSinceStart = _elapsedTime - _startTime;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (timeSinceStart < _nextBeatTimes[i]) continue;

                var currentCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / _currentBoardPoints[i]) + 1)];
                var prevCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / _currentBoardPoints[i]))];
                var currentSide = (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) % _currentBoardPoints[i]) + 1).ToString();
                //if (currentCycle == null || currentCycle[currentSide] == null) continue;

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                //Debug.Log(int.Parse(currentSide) + " / " + _nextBeatTimes[i] + " / " + _currentBoardPoints[i]);
                if (int.Parse(currentSide) == _currentBoardPoints[i] && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
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
                        currentCycle = _boardsData[i][Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / currentPoint)];
                        currentSide = (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                    }
                }

                _nextBeatTimes[i] += _beatIntervals[i];

                JSONNode currentBeat = currentCycle[currentSide];

                if (currentBeat["Camera"] != null)
                    {
                        JSONNode camera = currentBeat["Camera"];
                        if (camera["Position"] != null) _cameraManager.MoveCamera(camera["Position"][0], camera["Position"][1], camera["Easing"], camera["Duration"]);
                        if (camera["Rotation"] != null) _cameraManager.RotateCamera(camera["Rotation"], camera["Easing"], camera["Duration"]);
                        if (camera["Zoom"] != null) _cameraManager.ZoomCamera(camera["Zoom"], camera["Easing"], camera["Duration"]);
                    }
                if (!currentBeat["Beat"]) continue;
                
                float size = currentBeat["Size"] != null ? currentBeat["Size"].AsFloat : 1;
                Color color = new(
                    currentBeat["Color"]?[0]?.AsFloat ?? 1,
                    currentBeat["Color"]?[1]?.AsFloat ?? 1,
                    currentBeat["Color"]?[2]?.AsFloat ?? 1
                );
                if (color == Color.black) color = Color.white;
                float speed = currentBeat["Speed"] != null ? currentBeat["Speed"].AsFloat : 1;
                string easing = currentBeat["Easing"] != null ? currentBeat["Easing"] : "inoutcubic";
                _beatManager.CreateBeat(i, 1, _currentBoardSizes[i], _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm * 4, size, color, easing);
            }
        }
    }
}
